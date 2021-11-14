import asyncio
from typing import Dict
import websocket
from uuid import uuid4
from threading import Thread
from promise import Promise
import json
import time


class PromiseStore:
    """Manages async promise IDs"""

    def __init__(self):
        self.Promises: Dict[str, any] = {}

    def create(self):
        id = str(uuid4())

        def executor(resolve, reject):
            self.Promises[id] = {
                "id": id,
                "resolve": resolve,
                "reject": reject,
            }
        p = Promise(executor)
        self.Promises[id]["promise"] = p
        return id

    def complete(self, id: str, value: any):
        self.Promises[id]["resolve"](value)

    def fail(self, id: str, error: any):
        self.Promises[id]["reject"](error)

    def get(self, id: str):
        return self.Promises[id]["promise"]


class RaidToolkitClient:
    """Provides access to Raid Toolkit APIs"""

    def __init__(self, endpointUri="ws://localhost:9090"):
        self.Promises = PromiseStore()
        self.Connected = False
        self.ws = websocket.WebSocketApp(endpointUri,
                                         on_message=self._on_message,
                                         on_error=self._on_error,
                                         on_open=self._on_open)

    def connect(self):
        def run():
            self.ws.run_forever()

        Thread(target=run).start()

    def call(self, apiName: str, methodName: str, args=[]) -> Promise[any]:
        while self.Connected == False:
            time.sleep(0.01)

        promiseId = self.Promises.create()
        self.ws.send(json.dumps([
            apiName, "call", {
                "promiseId": promiseId,
                "methodName": methodName,
                "args": args,
            }
        ]))
        return self.Promises.get(promiseId)

    def _on_open(self, ws):
        self.Connected = True
        return

    def _on_message(self, ws, message):
        msg = json.loads(message)
        if msg[1] == "set-promise":
            self._set_promise(msg[2])
        return

    def _set_promise(self, value):
        if value["success"]:
            self.Promises.complete(value["promiseId"], value["value"])
        else:
            self.Promises.fail(value["promiseId"], value["error"])

    def _on_error(self, ws, error):
        print(error)
        return

    def _on_close(self, ws, close_status_code, close_msg):
        self.Connected = False
        return


async def main():
    websocket.enableTrace(True)
    client = RaidToolkitClient()
    client.connect()
    result = await client.call("account-api", "getAccounts")
    print(result)


if __name__ == "__main__":
    asyncio.run(main())
