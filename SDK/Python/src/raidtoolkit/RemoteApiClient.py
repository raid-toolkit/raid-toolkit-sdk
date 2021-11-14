from .PromiseStore import DeferredPromise, PromiseStore
import websocket
from threading import Thread
import json


class RemoteApiClient:
    """Provides access to Remote APIs"""

    def __init__(self, endpointUri):
        self.Promises = PromiseStore()
        self.Connected = DeferredPromise()
        self.ws = websocket.WebSocketApp(endpointUri,
                                         on_message=self._on_message,
                                         on_error=self._on_error,
                                         on_open=self._on_open)

    def connect(self):
        def run():
            self.ws.run_forever()

        self.worker = Thread(target=run)
        self.worker.start()

    def close(self):
        self.ws.close()

    async def call(self, apiName: str, methodName: str, args=[]):
        await self.Connected.promise

        promiseId = self.Promises.create()
        self.ws.send(json.dumps([
            apiName, "call", {
                "promiseId": promiseId,
                "methodName": methodName,
                "args": args,
            }
        ]))
        return await self.Promises.get(promiseId)

    def _on_open(self, ws):
        self.Connected.set_result(None)

    def _on_message(self, ws, message):
        msg = json.loads(message)
        if msg[1] == "set-promise":
            self._set_promise(msg[2])

    def _set_promise(self, value):
        if value["success"]:
            self.Promises.complete(value["promiseId"], value["value"])
        else:
            self.Promises.fail(value["promiseId"], value["error"])

    def _on_error(self, ws, error):
        return

    def _on_close(self, ws, close_status_code, close_msg):
        self.Connected = DeferredPromise()
