using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Options;
using NotificationService.Application.Interfaces;

namespace NotificationService.Infrastructure.Firebase;

/// <summary>
/// Firebase Cloud Messaging implementation.
/// </summary>
public class FcmService : IFcmService
{
    private readonly FirebaseMessaging _messaging;

    public FcmService(IOptions<FirebaseOptions> options)
    {
        // Initialize Firebase if not already done
        if (FirebaseApp.DefaultInstance == null)
        {
            var credentialsPath = options.Value.CredentialsPath;
            if (!string.IsNullOrEmpty(credentialsPath) && File.Exists(credentialsPath))
            {
                FirebaseApp.Create(new AppOptions
                {
                    Credential = GoogleCredential.FromFile(credentialsPath),
                    ProjectId = options.Value.ProjectId
                });
            }
            else
            {
                // Use Application Default Credentials (for GCP environments)
                FirebaseApp.Create(new AppOptions
                {
                    Credential = GoogleCredential.GetApplicationDefault(),
                    ProjectId = options.Value.ProjectId
                });
            }
        }

        _messaging = FirebaseMessaging.DefaultInstance;
    }

    public async Task<FcmResult> SendAsync(
        string token,
        string title,
        string body,
        string? imageUrl = null,
        Dictionary<string, string>? data = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var message = new Message
            {
                Token = token,
                Notification = new Notification
                {
                    Title = title,
                    Body = body,
                    ImageUrl = imageUrl
                },
                Data = data,
                Android = new AndroidConfig
                {
                    Priority = Priority.High,
                    Notification = new AndroidNotification
                    {
                        ClickAction = "FLUTTER_NOTIFICATION_CLICK"
                    }
                },
                Apns = new ApnsConfig
                {
                    Aps = new Aps
                    {
                        Badge = 1,
                        Sound = "default"
                    }
                }
            };

            var messageId = await _messaging.SendAsync(message, cancellationToken);
            return new FcmResult(true, messageId);
        }
        catch (Exception ex)
        {
            return new FcmResult(false, null, ex.Message);
        }
    }

    public async Task<FcmBatchResult> SendMulticastAsync(
        IReadOnlyList<string> tokens,
        string title,
        string body,
        string? imageUrl = null,
        Dictionary<string, string>? data = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var message = new MulticastMessage
            {
                Tokens = tokens.ToList(),
                Notification = new Notification
                {
                    Title = title,
                    Body = body,
                    ImageUrl = imageUrl
                },
                Data = data
            };

            var response = await _messaging.SendEachForMulticastAsync(message, cancellationToken);

            var results = response.Responses.Select((r, i) =>
                new FcmResult(r.IsSuccess, r.MessageId, r.Exception?.Message)).ToList();

            return new FcmBatchResult(response.SuccessCount, response.FailureCount, results);
        }
        catch (Exception ex)
        {
            var failedResults = tokens.Select(_ => new FcmResult(false, null, ex.Message)).ToList();
            return new FcmBatchResult(0, tokens.Count, failedResults);
        }
    }

    public async Task<FcmResult> SendToTopicAsync(
        string topic,
        string title,
        string body,
        string? imageUrl = null,
        Dictionary<string, string>? data = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var message = new Message
            {
                Topic = topic,
                Notification = new Notification
                {
                    Title = title,
                    Body = body,
                    ImageUrl = imageUrl
                },
                Data = data
            };

            var messageId = await _messaging.SendAsync(message, cancellationToken);
            return new FcmResult(true, messageId);
        }
        catch (Exception ex)
        {
            return new FcmResult(false, null, ex.Message);
        }
    }
}

public class FirebaseOptions
{
    public const string SectionName = "Firebase";
    public string ProjectId { get; set; } = string.Empty;
    public string CredentialsPath { get; set; } = string.Empty;
}
