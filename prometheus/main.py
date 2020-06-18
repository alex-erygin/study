import http.server
import random
from prometheus_client import start_http_server
from prometheus_client import Counter
import time

REQUESTS = Counter('requests_total', 'Requests total.')
EXCEPTIONS = Counter('exceptions_total', 'Exceptions total.')


class MyHandler(http.server.BaseHTTPRequestHandler):
    def do_GET(self):
        with EXCEPTIONS.count_exceptions():
            if random.random() < 0.2:
                raise Exception
        self.send_response(200)
        self.end_headers()
        self.wfile.write(b"Hell!")
        REQUESTS.inc()


if __name__ == "__main__":
    start_http_server(8000)
    server = http.server.HTTPServer(('localhost', 8001), MyHandler)
    server.serve_forever()
