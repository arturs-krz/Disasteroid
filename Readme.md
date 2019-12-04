# Disasteroid

## How to setup networking?
Open the Unity Asset Store (can do from the Unity app itself) and
search for **PUN2 - FREE** (Photon Networking free tier), add it and import it.
For the setup, it asks for the app ID. Ask me (Arturs) for it and I'll give it to you,
I don't want to put it on a public repo.
Additionally, select *Window -> Photon Unity Networking -> Highlight Server Settings*
and specify **Fixed Region: eu**, as well as **App Version**. Set this to some random version number.
If you have the same version number specified for your build as someone else working on the project,
and you both run at the same time, you might end up using the same game room and weird things can happen.

You can also run the networked version on the desktop. Run the desktop client **first**, before
opening up the mobile app, so the desktop client acts as the master client.