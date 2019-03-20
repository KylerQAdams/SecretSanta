# SecretSanta


<h2>Local Setup</h2>
<b>REQUIREMENTS</b>
<ul>
<li>Visual Studios 2017</li>
<li>.Net Framework 4.7+</li>
<li>IIS, IISExpress, or equivalent deployment environment</li>
</ul>

<b>SETUP</b>
<ol>
<li>Download the Repository to Local</li>
<li>Configure SecretSanta project to run on IIS equivalent 
  <ul>
    <li>Right click SecretSanta and select Properties at the bottom</li>
    <li>Select the Web tab</li>
    <li>Configure the settings under Servers to be compatible with the executing system</li>
  </ul></li> 
<li>Build the SecretSanta project</li>
<li>Run or Debug the SecretSanta project</li>
</ol>

<h2>Terms</h2>

<b>Participant</b>: A person who is partaking in the Secret Santa Exchange.

<b>Recipient</b>: A participant who in the current context is the one receiving the gift.

<b>Giver</b>: A participant who in the current context is the one giving the gift.

<b>Group [Participant's Group]</b>: One or more participants who should <b>not</b> be assigned as a recipient or giver to someone else in within that group.
(e.g. generally members of the same family or household group.)


<h2>General Usage</h2>
<b>Background</b>
The input is a list of groups.  Each group should contain one or more participant names.  
Participants will not be assigned as a recipient or as a giver to anyone else within their own group.
The output is a paired list of a Giver and a Recipient.

<b>Validation</b>
<ul>
<li>All participants must have a unique name.</li>
<li>There must be at least two participants.</li>
<li>There must be at least twice as many participants as the largest group of participants.</li>
<li>The number of participants must equal or be less than the configured maximum (default 10,000)</li>
</ul>
If the input fails to meet any of the above criteria, an error will be present and no list will generate.


<h2>Web Interface Usage</h2>
When you run or debug the project, a web browser should appear that loads up the website (as configured during Setup Step #2).  The web interface is the default page, with full url of "{Website}/Home/Index".

On the website is a portion of the prompt, a textbox, a button, instructions, and samples.  Adding text into a textbox will spawn additional textboxes such that there is always at least one empty textbox.

Each textbox represents a group.  For all participants of a group, enter that group's participant names in one single textbox seperated by a comma. (e.g. for the family of "John Doe", "Jane Doe" and "John Doe Jr."  the input would like like "John Doe, Jane Doe, John Doe Jr." in one textbox.)  Repeat this for all other groups, using a new textbox for each group.  All participants should be in a group, even if they are the only participant in that group.

<small>Alternatively, you may specify a new group is starting by using the semi-colon symbol => ;</small>

Once all participants are entered into the various textboxes, click "Generate Secret Santa List".  If the input is valid, a table with two columns: "Giver" and "Recipient" should appear below the button.  If the input is invalid, an error message will appear below the button.  Please fix the input and try again.

Additional instructions and sample inputs are present on the page.


<h2>REST API Usage</h2>
<ul>
<li><b>TYPE</b>: POST</li>
<li><b>URL</b>: {Website}/Home/GetSantasList</li>
<li><b>INPUT</b>: JSON: An array of objects containing an array of strings called Names.  Each object in the input is considered a group.  No one in the same "Names" array will be select as a recipient/giver.</li>
<li><b>OUTPUT</b>: JSON: An array of objects containing a Giver (string) and a Recipient (string)</li>
</ul>



<b>Sample JSON Input</b>


{"groups":[{"Names":["John Snow","Jane Snow","Jenny Snow"]},{"Names":["June Tobias","Judith Tobias"]},{"Names":["Jeremony Eins"]},{"Names":["Janet"]}]}


<b>Sample JSON Output</b>


[{"Giver":"John Snow","Recipient":"Jeremony Eins"},{"Giver":"Jane Snow","Recipient":"June Tobias"},{"Giver":"Jenny Snow","Recipient":"Janet"},{"Giver":"June Tobias","Recipient":"Jane Snow"},{"Giver":"Judith Tobias","Recipient":"Jenny Snow"},{"Giver":"Jeremony Eins","Recipient":"Judith Tobias"},{"Giver":"Janet","Recipient":"John Snow"}]





<h2>Original Prompt</h2>
<b>Secret Santa as a Service</b>

Secret Santa is a popular holiday tradition for a gift exchange within a group of people. Each person's name is written on a piece of paper and randomly selected one at a time by every person in the group to determine the recipient of their Secret Santa gift. However, physically drawing names is not always successful, since it is possible that a person could draw their own name or the name of their significant other.


Create a simple web application for choosing names for a SecretSanta gift exchange. This service should be accessible from a user interface via web browser and through a REST API. The service should allow for specifying the names of the individuals participating in the exchange and a mechanism that will allow the user to specify groups of 2 or more people that should be prevented from selecting each other (i.e.: prevent family members from selecting fellow family members). The result should be a list containing each participant's name and the name of their gift exchange recipient.


For purposes of this coding submission, you can use any languages/frameworks/libraries of your choice. For the sake of time, you do not have to implement any type of authentication or persist data. Please provide a Git repository of the solution along with instructions on how to run the solution locally, how to navigate to the app in the web browser, and how to consume the REST API.
