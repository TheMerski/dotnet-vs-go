import { check, sleep } from 'k6';
import { Trend } from 'k6/metrics';
import grpc from 'k6/net/grpc';
import { uuidv4 } from 'https://jslib.k6.io/k6-utils/1.4.0/index.js';

const TOTAL_TEST_TIME_S = parseInt(__ENV.TEST_TIME, 10);
const QUARTER_TEST_TIME_S = TOTAL_TEST_TIME_S / 4 + 's';

export const options = {
  scenarios: {
    static_data: {
      executor: 'ramping-arrival-rate',
      startRate: parseInt(__ENV.MAX_RPS / 5, 10),
      timeUnit: '1s',
      preAllocatedVUs: parseInt(__ENV.MAX_RPS / 10, 10),
      stages: [
        { duration: QUARTER_TEST_TIME_S, target: parseInt(__ENV.MAX_RPS / 2, 10) },
        { duration: QUARTER_TEST_TIME_S, target: parseInt(__ENV.MAX_RPS, 10) },
        { duration: QUARTER_TEST_TIME_S, target: parseInt(__ENV.MAX_RPS, 10) },
        { duration: QUARTER_TEST_TIME_S, target: 0 },
      ],
      exec: 'GetStaticData',
    },
    dynamic_data: {
      executor: 'ramping-arrival-rate',
      startRate: parseInt(__ENV.MAX_RPS / 20, 10),
      timeUnit: '1s',
      preAllocatedVUs: parseInt(__ENV.MAX_RPS / 10, 10),
      stages: [
        { duration: QUARTER_TEST_TIME_S, target: parseInt(__ENV.MAX_RPS / 4, 10) },
        { duration: QUARTER_TEST_TIME_S, target: parseInt(__ENV.MAX_RPS / 2, 10) },
        { duration: QUARTER_TEST_TIME_S, target: parseInt(__ENV.MAX_RPS, 10) },
        { duration: QUARTER_TEST_TIME_S, target: 0 },
      ],
      exec: 'GetDynamicData',
    },
  },
};

const client = new grpc.Client();
client.load(['../proto/generic/v1/'], ['generic.proto']);

const connectionTrend = new Trend('get_connection', true);
const dynamicTrend = new Trend('get_dynamic_data', true);
const staticTrend = new Trend('get_static_data', true);

export function GetDynamicData() {
  let startime = new Date();
  client.connect(__ENV.HOSTNAME, {
    plaintext: true,
  });
  connectionTrend.add(new Date() - startime);

  startime = new Date();
  const dynamicResp = client.invoke('generic.v1.GenericService/GetDynamicData', {
    request_id: uuidv4(),
  });
  check(dynamicResp, {
    'GetDynamicData is status OK': (r) => r && r.status === grpc.StatusOK,
  });
  dynamicTrend.add(new Date() - startime);

  client.close();
}

export function GetStaticData() {
  let startime = new Date();
  client.connect(__ENV.HOSTNAME, {
    plaintext: true,
  });
  connectionTrend.add(new Date() - startime);

  startime = new Date();
  const staticResp = client.invoke('generic.v1.GenericService/GetStaticData', {});
  check(staticResp, {
    'GetStaticData is status OK': (r) => r && r.status === grpc.StatusOK,
  });
  staticTrend.add(new Date() - startime);

  client.close();
}
