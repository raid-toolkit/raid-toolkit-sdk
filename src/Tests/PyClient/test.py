import json
import pprint
import asyncio
from raidtoolkit import RaidToolkitClient
from pygments import highlight, lexers, formatters
from termcolor import colored

def print_json(data):
    formatted_json = json.dumps(data, sort_keys=True, indent=2)
    colorful_json = highlight(formatted_json, lexers.JsonLexer(), formatters.TerminalFormatter())
    print(colorful_json)

async def main():
    client = RaidToolkitClient()
    client.connect()
    try:
        accounts = await client.get_connected_accounts()
        for account in accounts:
            print(colored(f'Account: {account.name}', 'yellow'))

            viewInfo = await account.get_current_view_info()
            print(colored(f'\tCurrent View: {viewInfo["viewKey"]} ({viewInfo["viewId"]})', 'green'))
            
            lastBattleResponse = await account.get_last_battle_response()
            print(colored(f'\tLast Battle Response: {lastBattleResponse["turnCount"]} turns', 'green'))

            heroes = await account.get_heroes()
            nLegendary = len(list(filter(lambda hero: hero["type"]["rarity"] == 'Legendary', heroes)))
            print(colored(f'\tHero count: {len(heroes)} ({nLegendary} legendary)', 'green'))

    finally:
        client.close()

if __name__ == "__main__":
    asyncio.run(main())