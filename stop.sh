#!/bin/bash
# Script to stop all Khet-360 frontend applications

echo "🛑 Stopping all Khet-360 frontend applications..."

# Kill processes by PID files
for pidfile in .tenant-erp.pid .tenant-mobile.pid .family-portal.pid .vendor-hub.pid .public-site.pid; do
  if [ -f "$pidfile" ]; then
    pid=$(cat "$pidfile")
    if [ -n "$pid" ] && kill -0 "$pid" 2>/dev/null; then
      kill "$pid"
      echo "Stopped process $pid (from $pidfile)"
    fi
    rm -f "$pidfile"
  fi
done

# Also kill any remaining processes on the ports
echo "🛑 Cleaning up any remaining processes..."
npx kill-port 5173 5174 5175 5176 5177 2>/dev/null || true

echo "✅ All applications stopped!"