# 🪙 KrakenPrice - Live Bitcoin Price on Your AIO

Turn your NZXT Kraken into a **live Bitcoin price display**! 

![Demo GIF](https://i.gyazo.com/5d255054737b567b1ebfa7a9ea0df9fe.gif)

## ✨ Features

- 🔴 **Live BTC prices** from multiple exchanges
- 🎨 **Beautiful animations** optimized for 640x640 displays
- 🚀 **Single executable** - no installation needed
- ⚡ **No API keys required** - works out of the box
- 🔄 **Auto-failover** - Binance → Coinbase → Kraken → Bitstamp
- 🌐 **Hosted option** - use without downloading anything

## 🎥 See it in action

[Demo Video](https://imgur.com/Jy4oKqg)

## 📦 Quick Start (2 minutes)

### 🌐 Option 1: Hosted Version (Easiest!)
No installation needed! Just paste this in NZXT CAM Web Integration:

https://krakenprice.runasp.net/

✅ Always online  
✅ Auto-updates  
✅ No setup required

### 💻 Option 2: Run Locally (Private & Faster)
1. **Download** the [latest release](https://github.com/rayman1337/krakenprice/releases)
2. **Run** `KrakenPrice.exe`
3. **Open NZXT CAM** → Web Integration
4. **Enter URL**: `http://localhost:8080`
5. **Done!** 🎉

### 👨‍💻 Option 3: For Developers
1. git clone https://github.com/rayman1337/krakenprice
2. cd krakenprice
3. dotnet run

## 💬 Contributing
Pull requests are welcome!  
If you’d like to add new features (extra coin support, new animations, etc.), just fork the repo and submit a PR.

## ⭐ Support
If you find KrakenPrice useful:
- Star the repo ⭐  
- Follow [@rayman_1337](https://x.com/rayman_1337) on Twitter/X for updates  
- Share screenshots of your Kraken display setup!

## 🛠️ Tech Stack
- **.NET 8 / C#**  
- **Minimal API** + **HttpClient**  
- **HttpClientFactory** for stable API calls and automatic failover  
- **HTML5 Canvas** for animations  

## 📝 License
MIT License – do whatever you want!
