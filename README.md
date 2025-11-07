# VMIX Multiview System

A sophisticated multiview management system for vMix that automatically distributes video sources across multiple displays with real-time status monitoring.

## 🚀 Features

- **Automated Source Distribution** - Automatically spreads sources across multiple multiview displays
- **Real-time Status Indicators** - Color-coded preview/program states
- **Dynamic Configuration** - Easy setup through central configuration panel
- **Scalable Architecture** - Supports unlimited multiview displays and sources
- **vMix Compatible** - Works with vMix 4K and Pro versions

## 🏗 System Architecture

MV_CONFIG (Control Panel) → Script Engine → Multiple MV Displays
↓
Source Assignment → Color Coding → Layer Distribution

### Core Components
- **MV_CONFIG** - Central configuration input
- **MV1, MV2, MV3...** - Multiview display outputs  
- **MV2_CONFIG** - Status display panel
- **VB Script** - Automation engine

## ⚡ Quick Start

### 1. Prerequisites
- vMix software (4K or Pro version)
- Basic understanding of vMix inputs and multiview

### 2. Installation
```bash
# Clone repository
git clone https://github.com/yourusername/vmix-multiview.git
# Or download script file directly
# MULTIVIEWER_V1.3.VB
```
3. Setup Steps
A. Add Multiview Templates
Open your vMix project

Add Color Inputs for each multiview (MV1, MV2, MV3...)

Design multiview layouts using vMix multiview designer

B. Create Configuration Input
Add Color Input named MV_CONFIG

Add text components:

txt_IN1.Text

txt_IN2.Text

txt_IN3.Text

... (as many as needed)

C. Deploy Script
Open vMix Scripting tab

Add new script → Import MULTIVIEWER_V1.3.VB

Start script

4. Configure Sources

5. txt_IN1.Text = "Camera1"
txt_IN2.Text = "Graphics"  
txt_IN3.Text = "Replay"

🎨 Color Coding System
Status	Background	Text Color
Program	🔴 Red	⚪ White
Preview	⚪ White	⚫ Black
Other	🔘 Gray	⚪ White
⚙️ Configuration
Customization Options
Edit these variables in the script:
```
' Colors (ARGB format)
Dim programBgColor As String = "#CC0000"    ' Red
Dim previewBgColor As String = "#FFFFFFFF"   ' White

' Timing
Dim refresh As Integer = 50                  ' Update frequency (ms)
In MV_CONFIG text fields, enter source names exactly as they appear in vMix:
```

Advanced Settings
Refresh Rate: Adjust update speed (default: 50ms)

Re-detection: Automatic re-scan every 30 seconds

Fallback Source: "BLACK" input for missing sources

🔧 Troubleshooting
Common Issues
Sources Not Appearing

Verify exact shortTitle names in MV_CONFIG

Check source inputs exist in vMix

Color Coding Not Working

Ensure MV_CONFIG input is properly detected

Verify text field naming convention

Script Not Starting

Check vMix scripting permissions

Verify no syntax errors in script

Debug Mode
Console outputs detection status:
```
Found / Найдено: 3 MVs, 10 layers / слоев
```

📊 Layer Distribution
The script intelligently distributes sources:

Even Distribution: 9 sources ÷ 3 MVs = 3 sources each

Remainder Handling: 10 sources ÷ 3 MVs = [4, 3, 3] distribution

🗂 File Structure

vmix-multiview/
├── MULTIVIEWER_V1.3.VB    # Main script file
├── Templates/             # vMix template files
├── Documentation/         # Technical documentation
└── Examples/             # Usage examples


🤝 Contributing
Fork the repository

Create feature branch (git checkout -b feature/improvement)

Commit changes (git commit -am 'Add new feature')

Push branch (git push origin feature/improvement)

Open Pull Request

📄 License
This project is licensed under the MIT License - see LICENSE file for details.

🆘 Support
📖 Detailed Documentation

🐛 Issue Tracker

💬 Discussions

Note: This system requires vMix software and basic understanding of vMix multiview functionality. Tested with vMix 4K/Pro versions.

