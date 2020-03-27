Basic Program Structure Overview
  - 
  DownUnder UI (name WIP?) is yet another GUI designer program. The goal of this project is to overtake all other GUI designers in quality, as well as visual appeal that wouldn't be possible with any other designer. This is why DownUnder is based on the game development oriented framework MonoGame, which leaves optimized special effects easily implemented. All elements of the framework need to be visually appealing past the bar set by other frameworks if this project is to compete.
  
  A wide selection of special effects are planned.
  
  For familiarity the framework is designed to resemble Microsoft's Visual Forms and Qt as closely as possible. DWindow is the base abstract class for all windows created, Widget is the base abstract class for all modular parts of the window, as per the traditional format. A new DWindow project will create a something that resembles a new MonoGame project very closely as well, as a DWindow inherits MonoGame's default Game class.

Formatting Notes
  -
  - int is not used for fields/properties defining anything spacial. float should be used due to scaling.
  - All values representing size/space are relative to pixels on a 1080p monitor. All values representing time are relative to a second.
  - All fields need to be protected/private, only public properties/methods.
  - No fields should be serializable/included in Clone().
  - Due to the complexity of UI related code, all UI related code should be seperated into regions. It should *generally* be Fields, Properties, Constructors, Methods, Events, and Cloning respectively. This is not strict but it does help. See Widget.cs as an example.
  - Comments, all non-generic classes/methods/properties/fields need a (single line /// \<summary\>). A comment is always prefered over no comment as an easy way to improve the code.
  - Methods should be static if they do not access anything not passed to them.
  - MonoGame.Extended.Size2 is generally never used due to being more incomplete than MonoGame.Extended.Point2 while having the same functionality. Vector2 is generally never used for class items either.
