Basic Program Structure Overview and Design Philosophy
  - 
  DownUnder UI (name WIP?) is yet another GUI designer program. The goal of this project is to overtake all other GUI designers in visual appeal that wouldn't be possible with any other designer. This is why DownUnder is based on the game development oriented framework MonoGame, which leaves optimized special effects easily implemented. All elements of the framework need to be visually appealing past the bar set by other frameworks if this project is to compete.
  
  A wide selection of special effects are planned. Nearly everything that moves or changes should have some kind of interpolation (implemented through the ChangingValue<> class), if even just a couple frames. Things in real life don't just pop into existence, why should they on your screen?
  
  For familiarity the framework is designed to resemble Microsoft's Visual Forms and Qt as closely as possible. DWindow is the base abstract class for all windows created, Widget is the base abstract class for all modular parts of the window, as per the traditional format. A new DWindow project will create a something that resembles a new MonoGame project very closely as well, as a DWindow inherits MonoGame's default Game class. As a result the framework does not interfere with or alter a MonoGame developer's normal workflow.

Formatting Notes
  -
  - int is not used for fields/properties defining anything spacial. float should be used due to scaling.
  - All values representing size/space are relative to pixels on a 1080p monitor. All values representing time are relative to a second.
  - All fields need to be protected/private, only public properties/methods.
  - No fields should be serializable/included in Clone().
  - A (single line /// \<summary\>) comment is always prefered over no comment. All public items should have a comment.
  - Methods should be static if they do not access anything not passed to them.
  - MonoGame.Extended.Size2 is generally never used due to being more incomplete than MonoGame.Extended.Point2 while having the same functionality. Vector2 is generally never used for class items either.
  - Opening brackets shouldn't get a line to themselves.
