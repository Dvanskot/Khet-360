#!/bin/bash
# Script to start all Khet-360 frontend applications

# Kill any existing processes on the ports we'll use
echo "🛑 Stopping any existing processes on ports 5173-5177..."
npx kill-port 5173 5174 5175 5176 5177 2>/dev/null || true

echo "🚀 Starting all Khet-360 frontend applications..."
echo ""

# Start all applications in background
npm run dev:tenant-erp &
TENANT_ERP_PID=$!
echo "Started Tenant-ERP (PID: $TENANT_ERP_PID)"

npm run dev:tenant-mobile &
TENANT_MOBILE_PID=$!
echo "Started Tenant-Mobile (PID: $TENANT_MOBILE_PID)"

npm run dev:family-portal &
FAMILY_PORTAL_PID=$!
echo "Started Family-Portal (PID: $FAMILY_PORTAL_PID)"

npm run dev:vendor-hub &
VENDOR_HUB_PID=$!
echo "Started Vendor-Hub (PID: $VENDOR_HUB_PID)"

npm run dev:public-site &
PUBLIC_SITE_PID=$!
echo "Started Public-Site (PID: $PUBLIC_SITE_PID)"

# Save PIDs for later cleanup
echo $TENANT_ERP_PID > .tenant-erp.pid
echo $TENANT_MOBILE_PID > .tenant-mobile.pid
echo $FAMILY_PORTAL_PID > .family-portal.pid
echo $VENDOR_HUB_PID > .vendor-hub.pid
echo $PUBLIC_SITE_PID > .public-site.pid

echo ""
echo "✅ All applications started!"
echo ""
echo "📋 Access URLs:"
echo "   Tenant-ERP:     http://localhost:5173"
echo "   Tenant-Mobile:  http://localhost:5174"
echo "   Family-Portal:  http://localhost:5175"
echo "   Vendor-Hub:     http://localhost:5176"
echo "   Public-Site:    http://localhost:5177"
echo ""
echo "📝 To stop all applications, run: ./stop.sh"
echo "💡 Tip: Use npm run dev:all to start all applications in parallel"