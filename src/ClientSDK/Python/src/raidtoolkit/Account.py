class Account:
    def __init__(self, client, account):
        self.client = client
        self.account = account
    
    @property
    def name(self):
        return self.account["name"]

    @property
    def id(self):
        return self.account["id"]

    def get_dump(self):
        return self.client.AccountApi.get_account_dump(self.id)

    def get_resources(self):
        return self.client.AccountApi.get_all_resources(self.id)
    
    def get_artifacts(self):
        return self.client.AccountApi.get_artifacts(self.id)

    def get_artifact_by_id(self, artifactId: str):
        return self.client.AccountApi.get_artifact_by_id(self.id, artifactId)

    def get_heroes(self, snapshot: bool = False):
        return self.client.AccountApi.get_heroes(self.id, snapshot)
    
    def get_hero_by_id(self, heroId: str, snapshot: bool = False):
        return self.client.AccountApi.get_hero_by_id(self.id, heroId, snapshot)
    
    def get_arena(self):
        return self.client.AccountApi.get_arena(self.id)
    
    def get_academy(self):
        return self.client.AccountApi.get_academy(self.id)
    
    def get_last_battle_response(self):
        return self.client.RealtimeApi.get_last_battle_response(self.id)
    
    def get_current_view_info(self):
        return self.client.RealtimeApi.get_current_view_info(self.id)
