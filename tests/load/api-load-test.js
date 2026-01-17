import http from 'k6/http';
import { check, sleep, group } from 'k6';
import { Rate, Trend } from 'k6/metrics';

// Custom metrics
const errorRate = new Rate('errors');
const feedLatency = new Trend('feed_latency');
const voucherLatency = new Trend('voucher_latency');

// Test configuration
export const options = {
    stages: [
        { duration: '30s', target: 10 },   // Ramp up to 10 users
        { duration: '1m', target: 50 },    // Ramp up to 50 users
        { duration: '2m', target: 50 },    // Stay at 50 users
        { duration: '30s', target: 100 },  // Spike to 100 users
        { duration: '1m', target: 100 },   // Stay at 100 users
        { duration: '30s', target: 0 },    // Ramp down
    ],
    thresholds: {
        http_req_duration: ['p(95)<500'],  // 95% of requests should be below 500ms
        http_req_failed: ['rate<0.01'],    // Less than 1% failure rate
        errors: ['rate<0.05'],             // Less than 5% error rate
    },
};

const BASE_URL = __ENV.BASE_URL || 'http://localhost:5000';

export default function () {
    group('Feed API', () => {
        const feedRes = http.get(`${BASE_URL}/api/posts/feed`);

        const feedCheck = check(feedRes, {
            'feed status is 200': (r) => r.status === 200,
            'feed has posts': (r) => {
                const body = JSON.parse(r.body);
                return body.posts && body.posts.length >= 0;
            },
        });

        errorRate.add(!feedCheck);
        feedLatency.add(feedRes.timings.duration);
    });

    sleep(1);

    group('Vouchers API', () => {
        const voucherRes = http.get(`${BASE_URL}/api/vouchers/active`);

        const voucherCheck = check(voucherRes, {
            'voucher status is 200': (r) => r.status === 200,
            'voucher has data': (r) => {
                const body = JSON.parse(r.body);
                return body.vouchers !== undefined;
            },
        });

        errorRate.add(!voucherCheck);
        voucherLatency.add(voucherRes.timings.duration);
    });

    sleep(1);

    group('Health Checks', () => {
        const services = ['auth', 'posts', 'vouchers'];

        for (const service of services) {
            const res = http.get(`${BASE_URL}/${service}/health`);
            check(res, {
                [`${service} health is 200`]: (r) => r.status === 200,
            });
        }
    });

    sleep(Math.random() * 2);
}

export function handleSummary(data) {
    return {
        'load-test-results.json': JSON.stringify(data, null, 2),
        stdout: textSummary(data, { indent: ' ', enableColors: true }),
    };
}

function textSummary(data, opts) {
    return `
=== Load Test Summary ===
Duration: ${data.state.testRunDurationMs / 1000}s
VUs Max: ${data.metrics.vus_max.values.max}

HTTP Requests:
  Total: ${data.metrics.http_reqs.values.count}
  Rate: ${data.metrics.http_reqs.values.rate.toFixed(2)}/s
  
Response Times (p95):
  ${data.metrics.http_req_duration.values['p(95)'].toFixed(2)}ms

Errors:
  ${(data.metrics.http_req_failed.values.rate * 100).toFixed(2)}%
`;
}
