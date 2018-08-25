import requests
import regex as re
import time
from datetime import datetime, timedelta

headers = {'User-Agent':'Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:57.0) Gecko/20100101 Firefox/57.0'}
postData = {"SessionLanguage":"English","HostelNumber":"89968","Username":"kevin","Password":"Panda2018**","Submit":"1"}
cookies = {}

def Login():
  print('Logining')
  loginURL = 'https://inbox.hostelworld.com/inbox/trylogin.php'
  login = s.post(loginURL, data = postData, headers = headers)
  cookies = login.cookies

def GetList():
  listURL = "https://inbox.hostelworld.com/booking/search/date?DateType=bookeddate&dateFrom=2018-01-24&dateTo=" + time.strftime('%Y-%m-%d')
  list = s.get(listURL, cookies = cookies, headers = headers)
  return re.findall('(?<=89968-)\d+',list.content)

def Get(ref):
  afterURL = "https://inbox.hostelworld.com/booking/view/" + str(ref)
  response = s.get(afterURL, cookies = cookies, headers = headers)
  html = response.content
  return html

def Rex(html,ref):
  rStr = []
  try:
    ref = re.search('(?<=Reference : 89968-)\d+',html).group(0)
  except:
    Login()
    Rex(Get(ref),ref)
    return
  rStr.append(ref)
  items = {}
  #anti-roomitems
  for item in re.finditer('(?<=<li><b>([^<]*?)</b></li><li[^>]*?>\s*)\S[^<>]*?(?=\s*</li>)',html):
    items[item.group(1)] = item.group(0)
  for item in ['Name','Email','Phone','Nationality','Booked','Source','Arriving','Arrival Time','Persons']:
    if (items.has_key(item)):
      rStr.append(items[item])
    else:
      rStr.append('')
  for item in re.findall('(?<=<li class=\x22cell\x22>\s*(?:<b>)*CNY )\S[^<>]*?(?=(?:</b>)*\s*</li>)',html):
    rStr.append(item)
  rStr.append(re.search('(?<=<span id=\x22bookingAckOn\x22>)[\s\S]*?(?=</span>)',html).group(0))
  mf.write(','.join(rStr) + '\n')
  #roomitems
  for room in re.findall('(?<=ul class=\x22title[\s\S]+?)<ul[\s\S]+?cell15[\s\S]+?</ul>',html):
    rStr = [ref]
    index = 0
    for item in re.findall('(?<=<li class=\x22[cell15]*?\x22>\s*)\S[\s\S]*?(?=\s*</li>)',room):
      rStr.append(item)
    rf.write(','.join(rStr) + '\n')

def SaveToExcel():
  s = requests.session()
  mf = open('hw.csv','w')
  rf = open('hw_room.csv','w')
  rCount = 0
  Login()
  for ref in GetList():
    rCount += 1
    print('Getting:' + str(rCount))
    Rex(Get(ref),ref)
  mf.close()
  rf.close()
