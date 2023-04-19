import asyncio
from concurrent.futures import Future
from uuid import uuid4


class DeferredPromise(Future):
    @property
    def promise(self):
        return asyncio.wrap_future(self)


class PromiseStore:
    """Manages async promise IDs"""

    def __init__(self):
        self.Promises: dict[str, DeferredPromise] = {}

    def create(self):
        id = str(uuid4())
        self.Promises[id] = DeferredPromise()
        return id

    def complete(self, id: str, value: any):
        self.Promises[id].set_result(value)

    def fail(self, id: str, error: any):
        self.Promises[id].set_exception(ValueError(error))

    def get(self, id: str):
        return self.Promises[id].promise
