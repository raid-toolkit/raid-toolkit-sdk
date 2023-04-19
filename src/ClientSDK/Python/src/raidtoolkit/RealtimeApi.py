from .RemoteApiClient import RemoteApiClient


class RealtimeApi:
    def __init__(self, client: RemoteApiClient):
        self.client = client

    def get_connected_accounts(self):
        return self.client.call("realtime-api", "getConnectedAccounts")

    def get_last_battle_response(self, accountId: str):
        return self.client.call("realtime-api", "getLastBattleResponse", [accountId])

    def get_current_view_info(self, accountId: str):
        return self.client.call("realtime-api", "getCurrentViewInfo", [accountId])
