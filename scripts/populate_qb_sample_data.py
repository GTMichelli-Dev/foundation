"""
Populate QuickBooks Desktop with sample Sand & Gravel company data.

Prerequisites:
  1. pip install pywin32
  2. QuickBooks Desktop must be running with your company file open
  3. When prompted by QuickBooks, grant this app access (choose "Yes, always")

Usage:
  python populate_qb_sample_data.py

This script creates:
  - Sample customers (contractors, municipalities, landscapers, etc.)
  - Sample products/inventory items (sand, gravel, topsoil, aggregates)
  - Sample vendors (quarries, equipment suppliers, fuel)
"""

import sys
import xml.etree.ElementTree as ET
from xml.sax.saxutils import escape as xml_escape

try:
    import win32com.client
except ImportError:
    print("ERROR: pywin32 is required. Install with: pip install pywin32")
    sys.exit(1)


# ---------------------------------------------------------------------------
# QuickBooks QBXML connection helpers
# ---------------------------------------------------------------------------

class QBConnection:
    """Manages a connection to QuickBooks Desktop via the QBXMLRP2 COM object."""

    def __init__(self):
        self.session_mgr = None
        self.ticket = None

    def connect(self, app_name="SandGravelSampleData"):
        self.session_mgr = win32com.client.Dispatch("QBXMLRP2.RequestProcessor")
        self.session_mgr.OpenConnection2("", app_name, 1)  # 1 = localQBD
        self.ticket = self.session_mgr.BeginSession("", 2)  # 2 = qbFileOpenDoNotCare
        print("Connected to QuickBooks Desktop.")

    def disconnect(self):
        if self.ticket:
            self.session_mgr.EndSession(self.ticket)
        if self.session_mgr:
            self.session_mgr.CloseConnection()
        print("Disconnected from QuickBooks Desktop.")

    def send_request(self, qbxml_request):
        """Send a QBXML request and return the parsed XML response."""
        response = self.session_mgr.ProcessRequest(self.ticket, qbxml_request)
        return response


def wrap_qbxml(inner_xml):
    """Wrap inner XML in the standard QBXML envelope."""
    return (
        '<?xml version="1.0" encoding="utf-8"?>'
        '<?qbxml version="16.0"?>'
        "<QBXML>"
        "<QBXMLMsgsRq onError=\"continueOnError\">"
        f"{inner_xml}"
        "</QBXMLMsgsRq>"
        "</QBXML>"
    )


def check_response(response_xml, operation_name):
    """Check a QBXML response for errors and print status."""
    root = ET.fromstring(response_xml)
    # Find any response element
    for elem in root.iter():
        status_code = elem.get("statusCode")
        if status_code is not None:
            status_msg = elem.get("statusMessage", "")
            if status_code == "0":
                print(f"  OK: {operation_name}")
                return True
            elif status_code == "3100":
                # Name already exists — not an error for our purposes
                print(f"  SKIP (already exists): {operation_name}")
                return True
            else:
                print(f"  ERROR [{status_code}]: {operation_name} - {status_msg}")
                return False
    return True


# ---------------------------------------------------------------------------
# Data definitions — Sand & Gravel company
# ---------------------------------------------------------------------------

CUSTOMERS = [
    {
        "name": "ABC Construction LLC",
        "company": "ABC Construction LLC",
        "first": "Mike",
        "last": "Henderson",
        "phone": "555-234-5678",
        "addr1": "1200 Industrial Blvd",
        "city": "Springfield",
        "state": "IL",
        "zip": "62704",
        "email": "mike@abcconstruction.com",
        "terms": "Net 30",
    },
    {
        "name": "Greenfield Landscaping",
        "company": "Greenfield Landscaping Inc",
        "first": "Sarah",
        "last": "Torres",
        "phone": "555-345-6789",
        "addr1": "450 Oak Street",
        "city": "Springfield",
        "state": "IL",
        "zip": "62701",
        "email": "sarah@greenfieldlandscaping.com",
        "terms": "Net 15",
    },
    {
        "name": "City of Springfield - Public Works",
        "company": "City of Springfield",
        "first": "James",
        "last": "Whitfield",
        "phone": "555-456-7890",
        "addr1": "100 Municipal Drive",
        "city": "Springfield",
        "state": "IL",
        "zip": "62702",
        "email": "jwhitfield@springfield.gov",
        "terms": "Net 30",
    },
    {
        "name": "Riverstone Builders",
        "company": "Riverstone Builders Corp",
        "first": "Tom",
        "last": "Nakamura",
        "phone": "555-567-8901",
        "addr1": "789 Commerce Parkway",
        "city": "Decatur",
        "state": "IL",
        "zip": "62521",
        "email": "tom@riverstonebuilders.com",
        "terms": "Net 30",
    },
    {
        "name": "Martin Paving Co",
        "company": "Martin Paving Company",
        "first": "Dave",
        "last": "Martin",
        "phone": "555-678-9012",
        "addr1": "3200 Highway 36 East",
        "city": "Jacksonville",
        "state": "IL",
        "zip": "62650",
        "email": "dave@martinpaving.com",
        "terms": "Net 30",
    },
    {
        "name": "Horizon Development Group",
        "company": "Horizon Development Group LLC",
        "first": "Rachel",
        "last": "Kim",
        "phone": "555-789-0123",
        "addr1": "555 Tower Road Suite 200",
        "city": "Springfield",
        "state": "IL",
        "zip": "62711",
        "email": "rachel@horizondev.com",
        "terms": "Net 30",
    },
    {
        "name": "Lakeview Concrete",
        "company": "Lakeview Concrete & Supply",
        "first": "Carlos",
        "last": "Ruiz",
        "phone": "555-890-1234",
        "addr1": "2100 Lake Shore Drive",
        "city": "Springfield",
        "state": "IL",
        "zip": "62707",
        "email": "carlos@lakeviewconcrete.com",
        "terms": "Net 15",
    },
    {
        "name": "Prairie Homebuilders",
        "company": "Prairie Homebuilders Inc",
        "first": "Linda",
        "last": "Nguyen",
        "phone": "555-901-2345",
        "addr1": "800 Meadow Lane",
        "city": "Chatham",
        "state": "IL",
        "zip": "62629",
        "email": "linda@prairiehomes.com",
        "terms": "Net 30",
    },
    {
        "name": "Tri-County Excavating",
        "company": "Tri-County Excavating LLC",
        "first": "Brett",
        "last": "Kowalski",
        "phone": "555-012-3456",
        "addr1": "4500 Pit Road",
        "city": "Sherman",
        "state": "IL",
        "zip": "62684",
        "email": "brett@tricountyexcavating.com",
        "terms": "Net 30",
    },
    {
        "name": "Heartland Asphalt",
        "company": "Heartland Asphalt and Paving Inc",
        "first": "Gary",
        "last": "Stenson",
        "phone": "555-123-4567",
        "addr1": "6700 Blacktop Lane",
        "city": "Rochester",
        "state": "IL",
        "zip": "62563",
        "email": "gary@heartlandasphalt.com",
        "terms": "Net 30",
    },
    {
        "name": "Sangamon County Highway Dept",
        "company": "Sangamon County Highway Department",
        "first": "Patricia",
        "last": "Dunlap",
        "phone": "555-234-5679",
        "addr1": "200 County Complex",
        "city": "Springfield",
        "state": "IL",
        "zip": "62702",
        "email": "pdunlap@sangamoncounty.gov",
        "terms": "Net 30",
    },
    {
        "name": "Cornerstone Ready Mix",
        "company": "Cornerstone Ready Mix Concrete",
        "first": "Anthony",
        "last": "Bello",
        "phone": "555-345-6780",
        "addr1": "1100 Batch Plant Drive",
        "city": "Auburn",
        "state": "IL",
        "zip": "62615",
        "email": "anthony@cornerstonereadymix.com",
        "terms": "Net 15",
    },
    {
        "name": "Iron Bridge Contractors",
        "company": "Iron Bridge Contractors Inc",
        "first": "Marcus",
        "last": "Tate",
        "phone": "555-456-7891",
        "addr1": "3300 Bridge Street",
        "city": "Riverton",
        "state": "IL",
        "zip": "62561",
        "email": "marcus@ironbridgecontractors.com",
        "terms": "Net 30",
    },
    {
        "name": "Flatland Grading",
        "company": "Flatland Grading and Hauling Co",
        "first": "Jesse",
        "last": "Hoffman",
        "phone": "555-567-8902",
        "addr1": "870 Dozer Road",
        "city": "Williamsville",
        "state": "IL",
        "zip": "62693",
        "email": "jesse@flatlandgrading.com",
        "terms": "Net 30",
    },
    {
        "name": "Stonewall Erosion Control",
        "company": "Stonewall Erosion Control LLC",
        "first": "Dana",
        "last": "Petrov",
        "phone": "555-678-9013",
        "addr1": "510 Levee Road",
        "city": "Petersburg",
        "state": "IL",
        "zip": "62675",
        "email": "dana@stonewallerosion.com",
        "terms": "Net 15",
    },
    {
        "name": "Capital City Concrete",
        "company": "Capital City Concrete Inc",
        "first": "Wayne",
        "last": "Decker",
        "phone": "555-789-0124",
        "addr1": "2200 Finisher Court",
        "city": "Springfield",
        "state": "IL",
        "zip": "62703",
        "email": "wayne@capitalcityconcrete.com",
        "terms": "Net 30",
    },
    {
        "name": "Aggregate Haulers Midwest",
        "company": "Aggregate Haulers Midwest LLC",
        "first": "Tina",
        "last": "Reeves",
        "phone": "555-890-1235",
        "addr1": "9200 Truck Terminal Blvd",
        "city": "Lincoln",
        "state": "IL",
        "zip": "62656",
        "email": "tina@aggregatehaulers.com",
        "terms": "Net 30",
    },
    {
        "name": "Lakeside Site Development",
        "company": "Lakeside Site Development Corp",
        "first": "Omar",
        "last": "Rashid",
        "phone": "555-901-2346",
        "addr1": "1450 Lakeshore Industrial Park",
        "city": "Taylorville",
        "state": "IL",
        "zip": "62568",
        "email": "omar@lakesidesite.com",
        "terms": "Net 30",
    },
]

# Products sold by weight (tons) — typical sand & gravel items
PRODUCTS = [
    {
        "name": "Fill Sand",
        "desc": "General purpose fill sand",
        "price": "12.50",
        "unit": "ton",
        "income_account": "Sales",
    },
    {
        "name": "Mason Sand",
        "desc": "Fine washed sand for masonry and mortar",
        "price": "18.00",
        "unit": "ton",
        "income_account": "Sales",
    },
    {
        "name": "Concrete Sand",
        "desc": "Washed sand for concrete mixing",
        "price": "16.00",
        "unit": "ton",
        "income_account": "Sales",
    },
    {
        "name": "Pea Gravel",
        "desc": "Small rounded gravel for drainage and landscaping",
        "price": "22.00",
        "unit": "ton",
        "income_account": "Sales",
    },
    {
        "name": "3/4 Crushed Stone",
        "desc": "3/4 inch crushed limestone aggregate",
        "price": "20.00",
        "unit": "ton",
        "income_account": "Sales",
    },
    {
        "name": "1-1/2 Crushed Stone",
        "desc": "1.5 inch crushed stone for base and drainage",
        "price": "18.50",
        "unit": "ton",
        "income_account": "Sales",
    },
    {
        "name": "CA-6 Grade 8 Limestone",
        "desc": "Compactable limestone for driveways and base",
        "price": "15.00",
        "unit": "ton",
        "income_account": "Sales",
    },
    {
        "name": "CA-7 Chip Rock",
        "desc": "Clean chip rock for drainage and fill",
        "price": "17.00",
        "unit": "ton",
        "income_account": "Sales",
    },
    {
        "name": "Rip Rap",
        "desc": "Large stone for erosion control and embankments",
        "price": "28.00",
        "unit": "ton",
        "income_account": "Sales",
    },
    {
        "name": "Screened Topsoil",
        "desc": "Screened black dirt topsoil",
        "price": "24.00",
        "unit": "ton",
        "income_account": "Sales",
    },
    {
        "name": "Road Gravel",
        "desc": "Mixed aggregate for road surface maintenance",
        "price": "14.00",
        "unit": "ton",
        "income_account": "Sales",
    },
    {
        "name": "Recycled Concrete",
        "desc": "Crushed recycled concrete aggregate",
        "price": "10.00",
        "unit": "ton",
        "income_account": "Sales",
    },
    {
        "name": "Hauling Fee",
        "desc": "Delivery/hauling charge",
        "price": "85.00",
        "unit": "ea",
        "income_account": "Sales",
    },
]

VENDORS = [
    {
        "name": "Midwest Quarry Supply",
        "company": "Midwest Quarry Supply Inc",
        "phone": "555-111-2222",
        "addr1": "9000 Quarry Road",
        "city": "Bloomington",
        "state": "IL",
        "zip": "61701",
    },
    {
        "name": "Central IL Fuel & Lubricants",
        "company": "Central IL Fuel & Lubricants LLC",
        "phone": "555-222-3333",
        "addr1": "400 Fuel Depot Lane",
        "city": "Springfield",
        "state": "IL",
        "zip": "62703",
    },
    {
        "name": "Heavy Equipment Rentals Inc",
        "company": "Heavy Equipment Rentals Inc",
        "phone": "555-333-4444",
        "addr1": "1500 Equipment Way",
        "city": "Peoria",
        "state": "IL",
        "zip": "61602",
    },
    {
        "name": "Prairie Parts & Service",
        "company": "Prairie Parts & Service Co",
        "phone": "555-444-5555",
        "addr1": "750 Shop Road",
        "city": "Springfield",
        "state": "IL",
        "zip": "62704",
    },
]


# ---------------------------------------------------------------------------
# QBXML request builders
# ---------------------------------------------------------------------------

def esc(d):
    """Return a copy of dict d with all string values XML-escaped."""
    return {k: xml_escape(v) if isinstance(v, str) else v for k, v in d.items()}


def build_customer_add(c):
    c = esc(c)
    return wrap_qbxml(f"""
    <CustomerAddRq>
      <CustomerAdd>
        <Name>{c['name']}</Name>
        <CompanyName>{c['company']}</CompanyName>
        <FirstName>{c['first']}</FirstName>
        <LastName>{c['last']}</LastName>
        <BillAddress>
          <Addr1>{c['addr1']}</Addr1>
          <City>{c['city']}</City>
          <State>{c['state']}</State>
          <PostalCode>{c['zip']}</PostalCode>
        </BillAddress>
        <Phone>{c['phone']}</Phone>
        <Email>{c['email']}</Email>
      </CustomerAdd>
    </CustomerAddRq>
    """)


def build_service_item_add(p):
    """Add a service item (non-inventory) — simplest item type."""
    p = esc(p)
    return wrap_qbxml(f"""
    <ItemServiceAddRq>
      <ItemServiceAdd>
        <Name>{p['name']}</Name>
        <SalesOrPurchase>
          <Desc>{p['desc']}</Desc>
          <Price>{p['price']}</Price>
          <AccountRef>
            <FullName>{p['income_account']}</FullName>
          </AccountRef>
        </SalesOrPurchase>
      </ItemServiceAdd>
    </ItemServiceAddRq>
    """)


def build_noninventory_item_add(p):
    """Add a non-inventory item — used for materials sold by weight."""
    p = esc(p)
    return wrap_qbxml(f"""
    <ItemNonInventoryAddRq>
      <ItemNonInventoryAdd>
        <Name>{p['name']}</Name>
        <SalesOrPurchase>
          <Desc>{p['desc']}</Desc>
          <Price>{p['price']}</Price>
          <AccountRef>
            <FullName>{p['income_account']}</FullName>
          </AccountRef>
        </SalesOrPurchase>
      </ItemNonInventoryAdd>
    </ItemNonInventoryAddRq>
    """)


def build_vendor_add(v):
    v = esc(v)
    return wrap_qbxml(f"""
    <VendorAddRq>
      <VendorAdd>
        <Name>{v['name']}</Name>
        <CompanyName>{v['company']}</CompanyName>
        <VendorAddress>
          <Addr1>{v['addr1']}</Addr1>
          <City>{v['city']}</City>
          <State>{v['state']}</State>
          <PostalCode>{v['zip']}</PostalCode>
        </VendorAddress>
        <Phone>{v['phone']}</Phone>
      </VendorAdd>
    </VendorAddRq>
    """)


def build_account_add(name, account_type):
    """Ensure an account exists (e.g., 'Sales' income account)."""
    return wrap_qbxml(f"""
    <AccountAddRq>
      <AccountAdd>
        <Name>{xml_escape(name)}</Name>
        <AccountType>{xml_escape(account_type)}</AccountType>
      </AccountAdd>
    </AccountAddRq>
    """)


# ---------------------------------------------------------------------------
# Main
# ---------------------------------------------------------------------------

def main():
    qb = QBConnection()

    try:
        qb.connect()

        # Ensure the Sales income account exists
        print("\n--- Creating Accounts ---")
        resp = qb.send_request(build_account_add("Sales", "Income"))
        check_response(resp, "Account: Sales")

        # Create customers
        print("\n--- Creating Customers ---")
        for c in CUSTOMERS:
            resp = qb.send_request(build_customer_add(c))
            check_response(resp, f"Customer: {c['name']}")

        # Create products (non-inventory items for materials, service for hauling)
        print("\n--- Creating Products/Items ---")
        for p in PRODUCTS:
            if p["unit"] == "ea":
                resp = qb.send_request(build_service_item_add(p))
            else:
                resp = qb.send_request(build_noninventory_item_add(p))
            check_response(resp, f"Item: {p['name']}")

        # Create vendors
        print("\n--- Creating Vendors ---")
        for v in VENDORS:
            resp = qb.send_request(build_vendor_add(v))
            check_response(resp, f"Vendor: {v['name']}")

        print("\nDone! Sample data has been added to QuickBooks.")

    except Exception as e:
        print(f"\nERROR: {e}")
        print("\nTroubleshooting:")
        print("  1. Make sure QuickBooks Desktop is running with a company file open")
        print("  2. When QuickBooks prompts to allow access, click 'Yes, always'")
        print("  3. Make sure you're running as the same Windows user that owns QB")
        sys.exit(1)

    finally:
        qb.disconnect()


if __name__ == "__main__":
    main()
