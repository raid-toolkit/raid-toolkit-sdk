from .AccountApi import AccountApi
from .RemoteApiClient import RemoteApiClient


class RaidToolkitClient:
    """Provides access to Raid Toolkit APIs"""

    def __init__(self, endpointUri="ws://localhost:9090"):
        self.client = RemoteApiClient(endpointUri)

    @property
    def AccountApi(self):
        return AccountApi(self.client)

    def connect(self):
        self.client.connect()

    def close(self):
        self.client.close()
