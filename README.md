# DiscordAccountManager

A lightweight and simple Discord multi-account manager that allows you to easily add, create, and switch between Discord accounts by opening them directly in your default web browser.

Perfect for users who manage multiple Discord accounts (communities, alt accounts, testing, etc.) without needing to log in/out constantly.
Created By Baconboy08.

### Features
- **Add Account** – Manually add Discord accounts using tokens (or login via browser and grab the token)
- **Create Account** – Quickly open Discord's registration page pre-filled or with captcha solvers in mind
- **Open Selected** – Instantly opens the selected account in your default web browser (Discord web version)
- Fast account switching with a clean interface
- Stores accounts in plain JSON format (unencrypted)
- Built in .NET 4.7.2 (C#) – Works on Windows

### Requirements
- **Windows 10 or Windows 11** (64-bit)
- **Visual Studio 2026** (Community edition is fine)
- **.NET Framework 4.7.2** (installed automatically with VS2026)
- **Google Chrome** (must be installed and set as default browser for optimal experience)

### Build Instructions (Important!)
The app **only works correctly in Release mode** due to Discord's anti-debugging protections when using tokens in the browser.

1. Open `DiscordAccountManager.sln` in Visual Studio 2026
2. At the top, change the build configuration from `Debug` → `Release`
3. Make sure the platform is set to `x86` or `x64` or `Any CPU`
4. Build → Build Solution (Ctrl+Shift+B)
5. The executable will be in:  
   `\Build\DiscordAccountManagerWPF.exe`

**Do NOT run in Debug mode** – Discord may block the token or show errors.

### How to Use
1. Run `DiscordAccountManagerWPF.exe` (from the Build folder)
2. Click **Add Account**
to open Discord login in browser
3. Optional: Use **Create Account** to open Discord signup in browser
4. Select any account from the list → click **Open Selected**
   → Google Chrome will open Discord already logged in as that account
