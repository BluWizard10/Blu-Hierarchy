# v1.8.2
- Added Icon for VRCFury Debug Info component. Only seen on Editor Test Copies of VRCFury-built Avatars.

# v1.8.1
- Updated Udon Behaviour icon and added icon for Udon Sharp Behaviours.

# v1.8.0
- The GameObject Toggles now support "Drag-to-Toggle", a power user feature that allows you to drag to toggle multiple GameObjects at once. Enabled by default.
  - To activate the function, hold ALT on your keyboard.
- Added button in Settings that resets all settings to the defaults.

# v1.7.3
- Added Tooltips when hovering over component icons. Will read the name of the component when hovered.

# v1.7.2
- Added Warning icon for GameObjects that contain Missing Scripts.
- Added `changelogUrl` argument to the package.json. This allows ALCOM (and future Creator Companion versions) to directly link to my changelogs.

# v1.7.1
- Added Layer Icon for `Item`, introduced in SDK 3.8.2.
- Updated Settings Panel with Tooltips to explain what the setting will do.
- Added setting that can Show/Hide the GameObject checkbox in the Hierarchy, because someone wanted that.
- Changed location of Settings panel back to `Tools/BluWizard LABS/Enhanced Hierarchy Settings`

# v1.7.0
- Added icon for VRCPerPlatformOverrides component.

# v1.6.0
- Component Icons no longer overlap text in the Hierarchy! Any icons that overlap the GameObject's name will now be faded away.

# v1.5.2
- Potential fix of a memory leak on Theme Change.
- Updated more Icons.

# v1.5.1
- Added new Icons for VRCFury SPS Components and Global Colliders.

# v1.5.0
- Added alternate Icons so that they are all easier to see in both the Unity Light and Dark Themes.
- Script has been updated with `isDarkTheme` parameter to detect these changes.

# v1.4.0
- Updated VRC Component Icons with neutral flat colors. This should make them slightly easier to see in both the Unity Light Theme and Dark Theme.
- Changed the name of `BluHierarchy Settings` to `Enhanced Hierarchy Settings`. This is only a visual change. Everything else remains the same.

# v1.2.0
- Components that have the `HideFlags.HideInInspector` argument in the Script will now be hidden by default. This fixes a bug where those hidden Components were exposed in the Hierarchy when they were not supposed to.
  - You can show them again in the Hierarchy by toggling "Show Hidden Components" in the Enhanced Hierarchy Settings, if for some weird reason you want to see them.

# v1.1.0
- Redesigned ALL icons for better clarity on the Hierarchy. The edgy file-like icons were getting a bit old...
- Added icon for d4rkAvatarOptimizer.

# v1.0.1
- Changed Menu Settings location to it's own dedicated area in the Unity Menu Bar, instead of inside `Tools`.

# v1.0.0
- Now going 1.0! Package was also renamed to coincide with the other Packages.
- Re-designed icons for VRC Constraint Components for visual clarity and to better differentiate between Unity vs VRC Constraints. The other ones were kind of messy to begin with...

# v0.7.0
- Added Icons for the new VRC Constraint Components.

# v0.6.1
- Added Icons for the new Unity Layers exposed by the latest VRChat SDK.

# v0.6.0
- Added the `GameObject -> Copy Path` option to test the usage of copying the Hierarchy path of a GameObject to the clipboard.

# v0.5.0
- Slightly widened all custom Component Icons for display clarity.
- Changed Udon Behaviour Component icon to something that I think looked better when representing either an Udon Graph or an UdonSharp Component.
- Added icon for the `VRCHeadChop` Component introduced in SDK 3.5.2.

# v0.4.2
- Added isPlaying check to prevent icons and GUI changes from loading while in Play Mode. This should optimize performance.

# v0.4.1
- Fixed a bug where the Layer Icons would never load when Unity is relaunched. It is refreshing the Icons with the most dirtiest method I could think of.

# v0.4.0
- Added Icons for VRChat-specific Layers that is appended next to the Component Icons. It is Off by default.
    - Enable this feature by Toggling On `Show Layer Icon` under `Tools -> BluWizard LABS -> Enhanced Hierarchy Settings` in Unity.
    - *Icon will only show if the GameObject is on a specific Layer other than the Default Layer.*

# v0.3.0
- Changed some Icons, again. Created a consistent look.
- Added more new Icons.
- Fixed missing #if statements.

# v0.2.2
- Changed some Icons for better visibility in the Hierarchy.
- Added new Icons for common Components such as Bakery

# v0.2.1
- Fixed Icon for VRCFury Component.

# v0.2.0
- Added Custom Icons for VRChat SDK Components.
- Added Custom Icon for VRCFury Component.