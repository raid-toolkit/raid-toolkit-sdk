from typing import Dict
import websocket
from uuid import UUID, uuid4
from threading import Thread
from promise import Promise
import json


class PromiseStore:
    """Manages async promise IDs"""
    Promises: Dict[UUID, Promise] = {}

    @classmethod
    def create(self):
        id = uuid4()
        self.Promises[id] = Promise()

    @classmethod
    def complete(self, id: UUID, value: any):
        self.Promises[id].resolve(value)

    @classmethod
    def fail(self, id: UUID, error: Exception):
        self.Promises[id].reject(error)

    @classmethod
    def get(self, id: UUID):
        return self.Promises[id]


class RaidToolkitClient:
    """Provides access to Raid Toolkit APIs"""

    def __init__(self, endpointUri="ws://localhost:9090"):
        self.Promises = PromiseStore
        self.Connected = Promise()
        self.ws = websocket.WebSocketApp(endpointUri,
                                         on_message=self._on_message,
                                         on_error=self._on_error,
                                         on_open=self._on_open)

    def connect(self):
        def run():
            self.ws.run_forever()

        Thread(target=run).start()
        Promise.wait(self.Connected, 1000)

    def call(self, apiName: str, methodName: str, args=[]):
        promiseId = self.Promises.create()
        self.ws.send(json.dumps([
            apiName, "call", {
                "promiseId": promiseId,
                "methodName": methodName,
                "args": args,
            }
        ]))

    def _on_open(self, ws):
        self.Connected.resolve(ws)
        return

    def _on_message(self, ws, message):
        print(message)
        return

    def _on_error(self, ws, error):
        return

    def _on_close(self, ws, close_status_code, close_msg):
        return


if __name__ == "__main__":
    websocket.enableTrace(True)
    client = RaidToolkitClient()
    client.connect()
    client.call("account-api", "getAccounts")
