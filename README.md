# Battleship State Tracker 

This is a <strong> Battleship State Tracking API</strong> which enables to simulate a battleship game with provision to <br>

- Create board<br>
- Add board to ship<br>
- Attack on the ship


## Technical Details

> Backend: .net core 3.0 <br>
> IDE: Visual Studio 2019 <br>
> Database: NA (no persistence layer)


## Build

- Open the solution in VS 2019
- Restore the nuget packages.
- Build and run the application which launches the localhost url<br>
 <a href="http://localhost:26476/api/tracker"> http://localhost:26476/api/tracker</a>
- Open the resclient like Postman or Insomnia and provide the details calling the required endpoint. <br>(refer to below section for endpoint detais)

## API endpoint details

#### CreateBoard Endpoint
***URL:*** <a href="http://localhost:26476/api/tracker/createboard">http://localhost:26476/api/tracker/createboard</a><br>
***Description:*** Creates a N X N battleboard with the given size(N) <br> 
***Request Method:*** POST <br>
***Request Body:*** <br>

		{
			"size" : 10
		}

***Response :***

<table style="width: 400px;">
    <thead>
        <tr>
	<th style="width: 35px;">Status Code</td></th>
        <th>Response Message</td></th>
        </tr>
    </thead>
    <tbody>
        <tr>
	        <td>201</td>
	        <td >NA</td>
        </tr>
         <tr>
	        <td>200</td>
	        <td >10X10 board already created</td>
        </tr>
        <tr>
	        <td>400</td>
	        <td >"errors": {
        "Size": [
            "Value should be between 1 and 50"
        ]
    }</td>
    </tbody>
</table>


#### AddShip Endpoint
***URL:*** <a href="http://localhost:26476/api/tracker/addship">http://localhost:26476/api/tracker/addship</a><br>
***Description:*** Adds ship to the board if the input is valid and ship is not already present on board <br> 
***Request Method:*** POST <br>
***Request Body:*** <br>

		{ 
		   "orientation":"horizontal",
		   "position":{ 
		      "x":8,
		      "y":8
		   },
		   "length":3
		}

Possible values for Orientation:<br>

- horizontal<br>
- vertical<br>


***Response :***

<table style="width: 400px;">
    <thead>
        <tr>
	<th style="width: 35px;">Status Code</td></th>
        <th>Response Message</td></th>
        </tr>
    </thead>
    <tbody>
        <tr>
	        <td>200</td>
	        <td > - Board is not created yet, please create board.<br>
				  - Cannot place the ship outside the board.<br>
				  - Ship is already placed on board.<br>
				  - Cell is already occupied by ship.<br>
				  - "EMPTY response when valid"</td>
        </tr>
        <tr>
	        <td>400</td>
	        <td >"Invalid field details"</td>
    </tbody>
</table>



#### Attack Endpoint
***URL:*** <a href="http://localhost:26476/api/tracker/attack">http://localhost:26476/api/tracker/attack</a><br>
***Description:*** Attack the ship by providing the coordinates, if a ship exists in that position then responds with a <strong>"HIT"</strong> else <strong>"MISS"</strong>. When the hit is the last block of the ship then <strong>"Hit, Ship drowned"</strong> is returned<br> 
***Request Method:*** POST <br>
***Request Body:*** <br>

		{
			"x":10,
			"y":8
		}


***Response :***

<table style="width: 400px;">
     <thead>
        <tr>
	<th style="width: 35px;">Status Code</td></th>
        <th>Response Message</td></th>
        </tr>
    </thead>
    </thead>
    <tbody>
        <tr>
	        <td>200</td>
	        <td > - Hit<br>
				  - Miss<br>
				  - Hit, Ship drowned.<br>
				  </td>
        </tr>
        <tr>
	        <td>400</td>
	        <td >"Invalid field details"</td>
    </tbody>
</table>
