# MCP Unity - Antigravity Connection Setup

## Overview
This document provides the configuration steps for connecting Google Antigravity to Unity via MCP (Model Context Protocol).

## Unity Side Configuration

### 1. Change Transport Mode
In the MCP Unity plugin settings window:
- Change **Transport Mode** from `Stdio` (default) to **`HTTP`** or **`SSE`**
- Reason: Antigravity and other MCP clients work more reliably with HTTP-based local servers

### 2. Select Client
- If a "Client" dropdown exists, select **`Antigravity`**

### 3. Configure Client
- Click **"Configure Client"** or **"Generate Config"** button
- This writes the Unity MCP server information to Antigravity's configuration file

### 4. Start Server
- Ensure the MCP server is running in Unity (usually auto-starts when the project is open)

## Antigravity Side Configuration

### 1. Detect MCP Server
- Open Antigravity's Agent panel
- Click the **"..."** menu → **"MCP Servers"** or **"Manage MCP Servers"**

### 2. Verify Connection
- Check if `unity` or `unity-mcp` appears in the server list
- If not listed, manually edit `mcp_config.json` and add the Unity MCP server HTTP endpoint

### 3. Restart Antigravity
- Completely restart Antigravity to apply configuration changes

## Troubleshooting

### Firewall
- If using HTTP transport, Windows Defender may show a warning
- Click **"Allow"** to permit communication

### Server Startup Order
- Recommended: **Unity (start MCP server)** → **Antigravity**

### Version Compatibility
- Antigravity is in preview and MCP specifications may change frequently
- Ensure the Unity MCP plugin is up-to-date

### Test Connection
After configuration, test the connection by asking Antigravity:
> "List all GameObjects in the current Unity scene"

If it responds with Unity scene data, the integration is successful.

## Current Project Status
- **MCP Plugin**: Installed (`MCPForUnity`)
- **Compilation**: Fixed (merge conflict in `Selection.cs` resolved)
- **Next Step**: Configure Transport Mode in Unity Editor → MCP Settings
