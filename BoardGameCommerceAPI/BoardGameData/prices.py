import requests
import numpy as np 
def get_price(name):
    url = f"https://boardgameprices.co.uk/api/search?search={name}&currency=GBP&page=1"
    print(url)
    response = requests.get(url)
    try: 
        price = round(response.json()["items"][0]["bestprice"],2)
    except:
        price = np.nan
    return price  #assume best match will be first 

import pandas as pd

pd.options.display.max_rows = 100 


df = pd.read_csv('boardgames_ranks.csv', comment='#',nrows = 1000)


df['price'] = df['name'].map(get_price)
final_df = df[["name", "yearpublished", "rank", "price"]]
final_df = final_df.dropna()
final_df.to_csv('boardgames_data.csv', index=False)