from .RemoteApiClient import RemoteApiClient


class RealtimeApi:
    def __init__(self, client: RemoteApiClient):
        self.client = client

    def get_connected_accounts(self):
        return self.client.call("realtime-api", "getConnectedAccounts")

    def get_last_battle_response(self):
        return self.client.call("realtime-api", "getLastBattleResponse")

    def get_current_view_info(self):
        return self.client.call("realtime-api", "getCurrentViewInfo")
