# Project Overview

Bones is the working name for a project management platform I'm working on. I've tried many project management platforms over the years, and none of them quite fit my needs for a one stop solution for everything I need to manage in my life.

Personal projects, tasks around the house, keeping track of the serial numbers of the various electronics I own, planning out and managing what I want to run on those electronics, etc. I want a single platform that can handle all of these in a single UI.

## Projects
This is the top level for everything in the application, project can be owned by either a User or an Organization.
Currently Organizations are mostly unimplemented, so everything pretty much relies on your account being the owner of it.

### Initiatives
The initiative is a grouping of task queues.

#### Task Queue
A list of tasks that need done, the order is currently based on the time added to the queue but this will eventually be configurable.

## Item Standards
All items should be versioned, with each version being an immutable snapshot of that version.
Due to this need each component of an item must be versioned as well, so items can continue to use their old versions when needed.
This includes: Layouts, Fields, and Assignments

## Item Fields
Any item field can be made required or not.

### Text Fields
Used for short text, can be displayed on a single line.

### Text Box Fields
Used for long text, should be a multi-line input.

### Integer Fields
The number should be an integer, optionally can be negative or restricted to positive only values.

### Decimal Fields
The number should be a decimal, optionally can be negative or restricted to positive only values.

### Boolean Fields
The value is yes or no, can be displayed as a single checkbox.

### DateTime Fields
A specific time and date, currently no option for only one of the two.

### ValueList Fields
A value that must be selected from a list of pre-defined values.

### Related Fields
A field used to relate items to other items, can either be a set list or a defined query based on values of other fields.
Not implemented currently, but for the Development planning example below I'd like to be able to select the specific features on bugs, then on that feature it would automatically show up as a Known Issue if the bug was still open.

## Item Types
### Asset
An asset is any long-lived item, it could be a server with IPs and serial numbers or pieces of documentation like how a feature in an application should work.

### Task (BonesTask in the code because C# said no)
A task is something you need to do, these are generally short lived but can stick around for longer if planning out far in the future. All tasks must belong to a queue.

## Assignment
Assignments use Assignment Slots, the slot can be configured for a single person to be assigned or many. The assignment can also be a role, which could be useful for examples like Features in the Development planning example below, if you have a role for Developers it makes sense to have them be the owners of a piece of code documentation.

## Item Layouts
An item layout defines the structure of an item by organizing its fields and assignment slots into a cohesive and ordered package.

### Field Organization
Both fields and assignment slots should be displayed in a specific order, and items should display their values following this same layout order.

### Layout Versioning

When a layout is updated, the system maintains backward compatibility:
- Existing items continue using their original layout version
- Users receive an alert showing an updated layout is available
- Migration is optional — users can choose to update at any time

During migration, users must acknowledge specific changes:
- Removed fields: User must acknowledge that field data will be deleted
- New required fields: User must provide values before the update completes
- This ensures the system is never at fault for unintended data loss

### Future Implications
Items can remain on different layout versions indefinitely, allowing
gradual adoption and preventing forced migrations.

## Item Processing
I haven't completely figured out how I want to do this yet, but I have a couple of ideas:
1. Some kind of 'visual programming' drag and drop system, similar to Unreal Engine, with predefined blocks of what they can do.
  - This is simpler, has less room for security errors, and would work for most things. But would be limited in its capabilities.
2. Go all out and embed some kind of small language like lua and let people do whatever they want with it.
  - Security nightmare, but would be far more flexible. Its also just cool.

## Example Use Cases
### Server documentation and management
As a IT admin I'd like to keep track of my servers, plan for future changes, and manage problems.

For Assets I would like:
- Server (SVR-*)
  - IP Address
  - Hardware Model
  - Hardware Serial
  - Operating System
  - Hosted Applications (Assets List | APP-*)
  - Known Issues (Tasks List | ISS-*)
- Application (APP-*)
  - Application Name
  - Packages
  - Configuration
  - Hosts (Assets List | SVR-*)
  - Known Issues (Tasks List | ISS-*)

For Tasks I would like:
- Issue (ISS-*)
  - Description
  - Issue with (Items List | Can be any items, Asset or Task)
    - For example, the issue could be with a CHG-* on a specific SVR-*.
- Change (CHG-*)
  - Need for change
  - Description of change
  - Rollback procedures
  - Change on (Assets List | SVR-* OR APP-*)

### Development planning
As a software developer, I'd like to track features in my application, the development of those features, and the bugs along the way.

For Assets I would like:
- Feature (FEAT-*)
  - Applicable to (End User, System Admin, Support Tech, etc.)
  - Need for feature
  - Feature description
  - Known Issues (Tasks List | BUG-*)

For Tasks I would like:
- Bug (BUG-*)
  - Feature (Asset | FEAT-*)
  - Is crash?
  - Reproduction Steps
  - Description
- User Story (STORY-*)
  - Feature (Asset | FEAT-*)
  - Story
  - Description
  - Required Tests

### Around the house planning
As someone who lives in a home, I have many things I need to do and keep track of.

For Assets I would like:
- Appliance (APL-*)
  - Brand
  - Purchase Date
  - Serial Number
  - Warrenty End Date
- Project (PRJ-*)
  - Project Goal
  - Expected Completion Date
  - Things to do (Tasks List | TSK-*)

For Tasks I would like:
- Project Task (TSK-*)
  - Project (Asset | PRJ-*)
  - What needs done?
  - Where does it need done?
  - When does it need to be done by?
  - What will it cost to do?
- Repeating Task (RETSK-*)
  - What needs done?
  - Where does it need done?
  - When does it need to be done by?
  - What will it cost to do?
  - How often does it need to repeat?
    - Unimplemented so far, but I'd like it to be possible to automatically create a new task when one is closed.