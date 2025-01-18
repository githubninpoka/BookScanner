<h1>Welcome to the Librarian</h1>

![afbeelding](https://github.com/user-attachments/assets/8a2b691f-d792-4331-a1e1-a6455eee5ceb)

The Librarian is a personal project that opens up a library of digital books that I've gathered over the years.

Right now you can search for single words:
<br/>
<ul>
	<li>Within a selection of books that I have found very useful (Reference)</li>
	<li>Within a selection of books that have tarot as their main topic</li>
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
<ul>
	<li>Esoteric texts are notoriously difficult to open up. There's a reason they're called esoteric texts.
	I don't think there are any solutions available that make this type of collection easily available.</li>
	<li>A study project. While studying many courses on C# and ASP.NET, the application of what I learnt into simple and small study projects just wasn't satisfactory. 
	Besides, I need the practice.
	So what this project has enabled me to apply:
		<li>a bit of separation of concerns. The solution is split up keeping in mind that UI and Logic should be separated:
			<li>a separate project for the Web UI in Blazor</li>
			<li>a separate project for the data access</li>
			<li>a separate project for loading the index</li>
			<li>a separate project for the Ebooks (models etc.)</li>
			</li>
		<li>Technology used:
			<li>Blazor (.Net8)</li>
			<li>OpenSearch</li>
			<li>Docker</li>
			</li>
		<li>Principles applied:
			<li>DRY</li>
			<li>SOLID:
				<li>S -> I've tried to keep the classes clean enough without scope creep.</li>
				<li>O -> a factory that allows for adding .DOCX and .TXT and others at a later stage.</li>
				<li>L -> Where possible I work with interfaces and not concrete implementations.</li>
				<li>I -> I've kept the interfaces as small as possible.</li>
				<li>D -> By depending on Interfaces where possible, with Dependency injection and also passing interfaces to constructors.</li>
				</li>
				</li>
		<li>Patterns that I could discern along the way:
			<li>factory</li>
			<li>options</li>
			<li>dependency injection</li>
			<li>builder</li>
			<li>and probably a couple more</li>
			</li>
		<li>ASP.NET common ways of working:
			<li>services registration</li>
			<li>appsettings</li>
			<li>Serilog</li>
			<li>PeriodicTimer</li>
			<li>Async with cancellationtokens</li>
			<li>Components, callbacks etc. in Blazor</li>
			<li>a bit of bootstrap</li>
			<li>etc.</li>
			</li>
	</li>
</ul>

			