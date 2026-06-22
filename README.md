# BluWizard's Enhanced Hierarchy System

My own personal editor enhancement system that introduces an improved Hierarchy look, tailored made for VRChat Creators. Contains a feature that allows toggling a GameObject or Component directly from the Hierarchy customized Icons representing common VRC SDK Components, a component tree showing it's connection to parent and child objects, and more!

Compared to other systems, this one is intended to match the Editor UI without being too distracting. Less is more, is how I would best put it.

![Demo Image](/Resources/Images/Unity_LgNzeXDKZ5.png)

# [📦 Add Enhanced Hierarchy System to Creator Companion](https://vpm.bluwizard.net/)

## Features:
- Component Icons. Click an Icon to toggle it's Active State On or Off.
- GameObject ActiveState Toggle Button. Click to toggle the GameObject On or Off.
  - You can also "Drag-to-Toggle" multiple GameObjects at once by holding down the ALT key.
  - Also works when animating!
- Custom Icons representing known Components from the VRChat SDK, such as Avatar Dynamics, Udon Behaviours, and VRC Constraints!
- Custom Icons for known third-party Components such as Bakery and VRCFury.
  - Components can also show their own Icons on the Component via Unity's native `[Icon(...)]` attribute.
- Tooltips when hovering over icons, telling you what they are at a glance.
- Alternate Icons with darker color tone if using the Light Theme.
- Relationship Lines showing an object's connection to it's parent and child objects.
- Settings Panel located in `Tools -> BluWizard LABS -> Enhanced Hierarchy Settings` to customize how the system should operate. Settings are persistent across Unity Projects.

## Benefits:
- No DLLs, no BS!
- Free!
- Open Source!

I've designed my Hierarchy System to be Free and Open Source. I did this because I feel it's only right for something like this... something that can make the lives of game development just that much easier to organize without having to pay for it, and without having to dissect a DLL just to add a new feature to it. That's the main reason for designing this system in the first place. Please use my code and edit it to how you want to use it... it's Open Source after all!

Please use the **Issues** tab if there's a new feature or a bug you wish to report. If you add new features, fixes, or other improvements to my system, **Pull Requests** are highly encouraged!

Thank you for using Enhanced Hierarchy and I hope you enjoy using it as much as I did designing it ^^

## License:
[MIT License](LICENSE.md)

Created with 💙 from BluWizard LABS.
