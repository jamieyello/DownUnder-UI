Formatting notes;
  - int is generally not used for fields/properties defining anything spacial. float should be used due to scaling.
  - All values representing size/space are relative to pixels on a 1080p monitor. All values representing time are relative to a second.
  - All fields need to be protected/private, only public properties/methods.
  - No fields should be serializable/included in Clone().
  - Due to the complexity of UI related code, all UI related code should be seperated into regions. See Widget.cs as an example.
