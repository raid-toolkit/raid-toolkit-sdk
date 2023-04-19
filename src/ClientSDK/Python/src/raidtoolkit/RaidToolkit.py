from .AccountApi import AccountApi
from .RemoteApiClient import RemoteApiClient
from .StaticDataApi import StaticDataApi
from .RealtimeApi import RealtimeApi
from .Account import Account


class RaidToolkitClient:
    """Provides access to Raid Toolkit APIs"""

    def __init__(self, endpointUri="ws://localhost:9090"):
        self.client = RemoteApiClient(endpointUri)

    async def get_all_accounts(self):
        accounts = await self.AccountApi.get_accounts()
        return [Account(self, account) for account in accounts]

    async def get_connected_accounts(self):
        accounts = await self.RealtimeApi.get_connected_accounts()
        return [Account(self, account) for account in accounts]

    @property
    def AccountApi(self):
        return AccountApi(self.client)

    @property
    def StaticDataApi(self):
        return StaticDataApi(self.client)

    @property
    def RealtimeApi(self):
        return RealtimeApi(self.client)

    def connect(self):
        self.client.connect()

    def close(self):
        self.client.close()
