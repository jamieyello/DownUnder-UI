Formatting notes;
  - int is not used for fields/properties defining anything spacial. float should be used due to scaling.
  - All values representing size/space are relative to pixels on a 1080p monitor. All values representing time are relative to a second.
  - All fields need to be protected/private, only public properties/methods.
  - No fields should be serializable/included in Clone().
  - Due to the complexity of UI related code, all UI related code should be seperated into regions. It should *generally* be Fields, Properties, Constructors, Methods, Events, and Cloning respectively. This is not strict but it does help. See Widget.cs as an example.
  - Comments, all non-generic classes/methods/properties/fields need a /// \<summary\>
  - Methods should be static if they do not access anything not passed to them.
