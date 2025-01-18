<h1>Welcome to the Librarian</h1>

![afbeelding](https://github.com/user-attachments/assets/8a2b691f-d792-4331-a1e1-a6455eee5ceb)

The Librarian is a personal project that opens up a library of digital books that I've gathered over the years.

Right now you can search for single words:
<br/>
<ul>
	<li>Within a selection of books that I have found very useful (Reference)</li>
	<li>Within a selection of books that have tarot as their main topic</ul>
	<li>Within all books (default)</li>
</ul>

The user can choose to:
<ul>
<li>Show results in normal or large text snippets</li>
<li>Search in a fuzzy way (Levenshtein distance algorithm)</li>
</ul>

HOW TO USE:
<ul>
<li>Just type a term, a single word and hit enter. A list of books containing the words should show up</li>
<li>Click a book in the resulting list to show all passages in that book</li>
</ul>

Prerequisites:
<ul>
<li>A working instance of OpenSearch</li>
</ul>

===============================================================================================================
Loading data:

The project EbooksIndex.UI is responsible for creating the proper index in OpenSearch and for loading all books into a common format.
Both PDF and EPUB formats are supported.

The BookReader project is an older console application that does the bare minimum. I've left it here for reference purposes but it shouldn't be used anymore.

===============================================================================================================
Purpose:

The project has two purposes.
	- Esoteric texts are notoriously difficult to open up. There's a reason they're called esoteric texts.
	I don't think there are any solutions available that make this type of collection easily available.
	- A study project. While studying many courses on C# and ASP.NET, the application of what I learnt into simple and small study projects just wasn't satisfactory. Besides, I need the practice.
	So what this project has enabled me to apply:
		- a bit of separation of concerns. The solution is split up keeping in mind that UI and Logic should be separated:
			- a separate project for the Web UI in Blazor
			- a separate project for the data access
			- a separate project for loading the index
			- a separate project for the Ebooks (models etc.)
		- Technology used:
			- Blazor (.Net8)
			- OpenSearch
			- Docker
		- Principles applied:
			- DRY
			- SOLID:
				- S -> I've tried to keep the classes clean enough without scope creep.
				- O -> a factory that allows for adding .DOCX and .TXT and others at a later stage.
				- L -> Where possible I work with interfaces and not concrete implementations.
				- I -> I've kept the interfaces as small as possible.
				- D -> By depending on Interfaces where possible, with Dependency injection and also passing interfaces to constructors.
		- Patterns that I could discern along the way:
			- factory
			- options
			- dependency injection
			- builder
			- and probably a couple more
		- ASP.NET common ways of working:
			- services registration
			- appsettings
			- Serilog
			- PeriodicTimer
			- Async with cancellationtokens
			- Components, callbacks etc. in Blazor
			- a bit of bootstrap
			- etc.


			