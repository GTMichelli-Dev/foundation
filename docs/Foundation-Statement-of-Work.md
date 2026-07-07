# Statement of Work

## Foundation — Truck Scale Management System

**Prepared for:** [Client Name]
**Prepared by:** [Your Company Name]
**Date:** [Date]
**Document version:** 1.0

---

## 1. Executive Summary

**Foundation** is a modern, web-based truck scale management platform that turns
your weighbridge into a fully digital operation. It captures live weights from
your scale, records inbound and outbound transactions, prints tickets at the
scale house, and produces the reports your office and customers need — all from
a single browser-based application that runs on-site or in the cloud.

Foundation replaces manual weigh tickets, spreadsheets, and disconnected legacy
software with one integrated system. Operators weigh trucks in and out on a
touchscreen; managers pull revenue and tonnage reports in seconds; and the whole
site — scale, cameras, printers, and accounting — is tied together in real time.

This Statement of Work describes what Foundation does, how it operates, what is
delivered, and the options available to tailor it to your site.

---

## 2. What Foundation Does

At its core, Foundation manages the full lifecycle of a truck weighing
transaction:

| Stage | What happens |
|-------|--------------|
| **Truck arrives** | Operator (or an unattended kiosk) selects the truck, customer, carrier, and commodity, then captures the live weight from the scale as the **weigh-in**. |
| **Truck loads / unloads** | The truck is tracked on-site in the "Inbound Trucks" list until it returns to the scale. |
| **Truck departs** | The operator captures the **weigh-out**. Foundation automatically calculates the **net weight** and generates a numbered ticket. |
| **Ticket prints** | The finished ticket prints instantly at the scale house on a thermal or standard printer. |
| **Data flows** | The completed transaction is stored, available for reporting, and (optionally) pushed to your accounting system as an invoice. |

Every transaction is stored with its customer, carrier, truck, commodity,
location, destination, weights, timestamps, and ticket number — giving you a
complete, searchable history of everything that has crossed your scale.

---

## 3. Core Capabilities

- **Real-time scale display** — Live weight readings stream from the connected
  scale directly to the screen, with motion and stability indicators so
  operators know exactly when a reading is valid.

- **Weigh-in / weigh-out workflow** — Fast, guided capture of inbound and
  outbound weights with automatic net-weight calculation. Supports stored
  ("retained") tare weights so regular trucks can be weighed in a single pass.

- **Live truck tracking** — See at a glance which trucks are currently on-site
  (weighed in, not yet out) and which transactions are complete.

- **Reporting & exports** — Filter by date range and group by customer, carrier,
  commodity, and more. Export any report to **Excel** or **PDF** for the office,
  auditors, or customers.

- **Master data management** — Maintain your lists of Customers, Carriers,
  Trucks, Commodities, Locations, and Destinations in one place, so operators
  select from clean, consistent data instead of retyping it.

- **Touchscreen kiosk mode** — A simplified, touchscreen-optimized interface for
  unattended or driver-operated scale houses. You control which prompts the
  driver sees.

- **On-site ticket printing** — Tickets print automatically at one or more scale
  houses. Printers can be driven directly, through the browser, or through a
  small remote print agent (see Architecture).

- **Customizable ticket layouts** — A built-in ticket designer lets you adjust
  the printed ticket — logo, header lines, fields, and layout — without a
  developer.

- **Optional user logins & roles** — Turn on secure logins with three role
  levels (Operator, Manager, Administrator) when you need access control; leave
  it off for a simple single-station setup.

- **Branding & configuration** — Company name and address on tickets, custom
  icon, selectable themes, and configurable kiosk prompts.

- **Demo mode** — A built-in scale simulator lets you train staff and evaluate
  the system with no scale hardware connected.

---

## 4. How Foundation Operates (System Architecture)

Foundation is built as a central web application surrounded by lightweight
"edge" services that connect it to your physical hardware. This design keeps the
core system simple and reliable while letting it talk to real scales, cameras,
printers, and accounting software.

```
                    ┌─────────────────────────────────────┐
                    │        Foundation Web Application     │
                    │  (runs on a local server or cloud VM) │
                    │                                       │
   Browsers /       │   • Weigh-in / weigh-out engine       │
   Kiosks  ───────► │   • Transaction & master data store   │
                    │   • Reporting & ticket generation     │
                    │   • Real-time hub (live updates)      │
                    └───────────────┬───────────────────────┘
                                    │  (secure real-time link)
        ┌───────────────┬───────────┼───────────────┬───────────────┐
        ▼               ▼           ▼               ▼               ▼
  Scale Reader     Print Agent   Print Agent    Camera Service   Accounting
  (reads the       (Scale        (Scale         (captures        Sync
   weighbridge)     House A)      House B)        photos)         (invoices)
```

### The central web application
The heart of the system. It serves the operator screens and kiosk, stores all
transactions and master data, generates tickets, and runs the reports. It can be
hosted **on a local server on your network** or on a **secure internet-facing
cloud server** — your choice.

### The edge services (connect Foundation to your hardware)
Each of these is a small, self-healing program that maintains a live connection
to the web application and automatically reconnects if the network drops:

- **Scale Reader Service** — Reads live weight from your weighbridge indicator
  over the network and streams it to the application in real time. Supports
  common industrial scale protocols and multiple scales from one service.

- **Print Agent** — Receives finished tickets and prints them at the scale
  house, on Windows or Linux/Raspberry Pi printers. One agent per scale house
  lets you print inbound and outbound tickets at different locations.

- **Camera Capture Service** *(optional)* — Snaps a photo of the truck at the
  moment of weighing from an IP or USB camera and attaches it to the
  transaction, for dispute resolution and security.

- **Accounting Sync Service** *(optional)* — Two-way sync with QuickBooks
  Desktop: pulls in customers and items, and pushes completed tickets back as
  invoices, eliminating double entry.

Because the edge services make an **outbound** connection to the web
application, they work behind firewalls without special network configuration,
and the system keeps running even across brief network interruptions.

---

## 5. Deployment Options

Foundation can be deployed in the way that best fits your site and IT
environment. The two standard options are:

| Option | Best for | Access |
|--------|----------|--------|
| **On-premise / LAN** | A single site where operators are on the same local network and no outside access is needed. | Reachable at a local address on your network. |
| **Cloud-hosted (HTTPS)** | Multi-site access, remote managers, or where you want the system reachable securely from anywhere with an encrypted (SSL) connection. | Reachable at your own secure web address. |

Unattended kiosk displays (e.g., a screen wired to the scale house that boots
straight into the driver interface and self-recovers after a power or network
outage) are supported as an add-on for driver-operated lanes.

---

## 6. Scope of Work

The following describes the standard implementation deliverables. Optional
modules are listed separately in Section 7.

### 6.1 In Scope — Standard Implementation

1. **Installation & configuration** of the Foundation web application on the
   agreed hosting environment (on-premise or cloud).
2. **Scale integration** — installation and configuration of the Scale Reader
   Service against your existing weighbridge indicator.
3. **Printer setup** — configuration of on-site ticket printing at one scale
   house.
4. **Ticket design** — configuration of one ticket layout with your company
   branding, header lines, and required fields.
5. **Master data setup** — initial loading of your Customers, Carriers, Trucks,
   Commodities, Locations, and Destinations (from data provided by the client).
6. **User & role configuration** — setup of logins and roles if required.
7. **Verification & acceptance testing** — end-to-end test of a live weigh-in /
   weigh-out cycle, ticket print, and reporting.
8. **Operator training** — a training session covering daily operation,
   reporting, and master data maintenance.
9. **Handover documentation** — administration and operating reference for your
   team.

### 6.2 Out of Scope (unless added as an option)

- Supply or repair of scale, indicator, printer, camera, or network hardware.
- Custom software features not described in this document.
- Data migration from legacy systems beyond the initial master data load.
- Ongoing hosting fees, domain registration, or third-party licensing not
  specified herein.
- Integrations other than those explicitly listed in Section 7.

---

## 7. Optional Modules & Add-Ons

These can be included at the outset or added later as your operation grows:

| Module | What it adds |
|--------|--------------|
| **Additional scale houses / printers** | Print and weigh at multiple locations from one system. |
| **Camera capture** | Photo of each truck attached to its transaction. |
| **Accounting integration** | Two-way QuickBooks Desktop sync (customers, items, invoices). |
| **Unattended kiosk display** | Driver-operated, self-recovering touchscreen lane. |
| **Multi-scale support** | Read from more than one weighbridge simultaneously. |
| **Additional ticket layouts** | Different ticket formats per commodity, customer, or site. |

---

## 8. Implementation Approach

A typical Foundation deployment follows these phases:

1. **Discovery & confirmation** — Confirm scale indicator model, printer(s),
   network layout, hosting choice, and ticket requirements.
2. **Provisioning** — Stand up the hosting environment and install the web
   application.
3. **Integration** — Connect the scale, printer(s), and any optional services.
4. **Configuration** — Load master data, design the ticket, and set company
   branding and options.
5. **Testing & acceptance** — Run live weigh cycles and confirm results against
   the acceptance criteria (Section 9).
6. **Training & go-live** — Train staff and transition to live operation.
7. **Support handover** — Provide documentation and agreed support terms.

---

## 9. Acceptance Criteria

The implementation is considered complete and accepted when:

- A truck can be weighed in and out, and Foundation calculates the correct net
  weight.
- A ticket prints correctly at the scale house with the agreed layout and
  branding.
- Completed transactions appear in the transaction history and in reports.
- A report can be filtered, grouped, and exported to Excel and PDF.
- Any contracted optional modules (Section 7) operate as described.

---

## 10. Assumptions & Client Responsibilities

- The client provides a functioning, calibrated scale and indicator with network
  connectivity.
- The client provides printer(s), cameras, and networking hardware as needed.
- The client supplies initial master data (customers, carriers, trucks, etc.) in
  a usable format.
- The client provides remote or on-site access as required for installation.
- For cloud hosting, the client provides or authorizes the domain name and
  associated hosting arrangements.

---

## 11. Support & Maintenance

Post-implementation support options can be tailored to your needs, including
software updates, remote assistance, and response-time commitments. Support
terms, duration, and pricing are defined in the accompanying commercial
proposal.

---

## 12. Technology Summary

For your IT team's reference, Foundation is built on a modern, well-supported
technology stack:

- **Application platform:** ASP.NET Core 8 web application (cross-platform).
- **Database:** SQLite by default, with support for MariaDB/MySQL for larger
  deployments.
- **Real-time communication:** Secure WebSocket-based live updates between the
  application, scales, printers, and cameras.
- **Reporting engine:** Professional report and ticket designer with Excel and
  PDF export.
- **Hosting:** Runs on Windows or Linux servers, on-premise or in the cloud;
  edge services run on Windows, Linux, or Raspberry Pi devices.

---

*This Statement of Work is provided for evaluation purposes. Final scope,
pricing, timeline, and terms are set out in the accompanying commercial proposal
and agreement.*
