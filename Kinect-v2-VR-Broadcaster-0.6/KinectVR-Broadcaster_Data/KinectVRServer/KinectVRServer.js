// --------------------------------------------------- STATIC VARIABLES
// local IP address
var HOST = '192.168.43.209';
HOST = getIPAddress();
// internet IP address
var PUBHOST = '104.131.11.131';
// port for client connections
var DEVICEPORT = 5000;
// clients array
var clients = new Array();
// =================================================== END STATIC VARIABLES

// --------------------------------------------------- INCLUDES
//TCP socket for device connections
var net = require('net');


//Repeater

var repeat = require('repeat');

// =================================================== END INCLUDES





// --------------------------------------------------- RUN ON START
// connect to MYSQL database
/*
connection.connect(function(err) {
  if (!err)
  console.log('CONNECTED to databse');
});
*/
// =================================================== END RUN ON START

// --------------------------------------------------- SOCKET SERVER LISTENERS
net.createServer(function(sock) {
    // We have a connection - a socket object is assigned to the connection automatically
    sock.setEncoding("utf8")
    var ip = sock.remoteAddress;
    var port = sock.remotePort;
	console.log("Connected "+ip+":"+port);

    sock.on('data', function(data) {
	Parser(sock,ip,port,data);
    });
    // Add a 'close' event handler to this instance of socket
    sock.on('close', function(data) {
	DeleteClient(ip,port);
    });
    sock.on('error', function(err){
       DeleteClient(ip,port);
    });
}).listen(DEVICEPORT, HOST);
// =================================================== END SOCKET SERVER LISTENERS

// --------------------------------------------------- REPEATERS
repeat(Broadcast).every(16, 'ms').start();
repeat(BroadcastBodyList).every(1000, 'ms').start();
// =================================================== END REPEATERS

// --------------------------------------------------- FUNCTIONS

function getIPAddress() {
  var interfaces = require('os').networkInterfaces();
  for (var devName in interfaces) {
    var iface = interfaces[devName];

    for (var i = 0; i < iface.length; i++) {
      var alias = iface[i];
      if (alias.family === 'IPv4' && alias.address !== '127.0.0.1' && !alias.internal)
        return alias.address;
    }
  }

  return '0.0.0.0';
}




function Parser (sock,ip,port,data){
var encap = data.split("*"[0]);
for (var i = 0; i<encap.length; i++){
	if (encap[i] != ""){
		var s = encap[i].split(","[0]);
		if (s[0] == "login"){
		InsertClient(sock,ip,port,s[1]);
		}
		if (s[0] == "cbody"){
		// Create Body
		CreateBody (s[1]);
		}
		if (s[0] == "dbody"){
		// Remove Body
		RemoveBody (s[1]);
		}
		
		if (s[0] == "ubody"){
		// Update Body
		UpdateBodyPart (s[1],s[2],s[3],s[4],s[5],s[6],s[7],s[8],s[9],s[10]);
		
		}
		
	}
}
}


bodies = new Array();

function CreateBody(id){
var check = false;
var tempid;
	for (var i = 0; i<bodies.length; i++){
		if (bodies[i].id == id){
		 check = true;
		 break;
		}else{
		check = false;
		}
	}
	if (check == false){
		
		var newbody = new Body(id);
		if (id.length > 1)
		newbody.setParts();
		else
		newbody.setPartsV1();
		
		bodies.push( newbody );
		SendAllClients("*1|"+id+"*");
		//console.log(newbody.parts[0].id);
	}
}

function RemoveBody (id){
	for (var i = 0; i<bodies.length; i++){
		if (bodies[i].id == id){
		 bodies.splice(i,1);
		 
		 SendAllClients("*0|"+id+"*");
		 console.log("Removed Body " + id);
		 break;
		}
	}
}

function SendAllClients(data){
  for (var i = 0; i<clients.length; i++){
  if (clients[i] != null)
  clients[i].sock.write(data);
  }	

}

function UpdateBodyPart(id,jointName,posx,posy,posz,rotx,roty,rotz,rotw, infered){
	

	var body = idFind(id,bodies);

	if (body){
	var joint = idFind(jointName,body.parts);
	if (joint){
	
	joint.position.x = posx;
	joint.position.y = posy;
	joint.position.z = posz;
	
	joint.rotation.x = rotx;
	joint.rotation.y = roty;
	joint.rotation.z = rotz;
	joint.rotation.z = rotw;
	joint.infered = infered;
	//console.log("Updating Joint " + jointName); 
	}
	}

}



function idFind(id,array){
	for (var i = 0; i<array.length; i++){
		if (array[i].id == id){
		 return array[i];
		}
	}
}

function Broadcast() {



for (var j = 0; j<bodies.length; j++){
	var data = "*2|" + bodies[j].id + "|";
	for (var r = 0; r<bodies[j].parts.length; r++){
		data += bodies[j].parts[r].id + "," + bodies[j].parts[r].position.x + "," + bodies[j].parts[r].position.y + "," + bodies[j].parts[r].position.z + ",";
		data += bodies[j].parts[r].rotation.x + "," + bodies[j].parts[r].rotation.y + "," + bodies[j].parts[r].rotation.z + "," + bodies[j].parts[r].rotation.w + "," + bodies[j].parts[r].infered + "|";
	
	}
	data += "*";
	
  for (var i = 0; i<clients.length; i++){
  if (clients[i] != null)
  clients[i].sock.write(data);
  }	
	
}




}

function BroadcastBodyList(){


var bodylist = "";
 for (var x = 0; x < bodies.length; x++){
	if (bodies[x]){
		
		if (x == bodies.length - 1){
		bodylist += bodies[x].id;
		}else{
		bodylist += bodies[x].id+",";
		}
	}
 }

	for (var i = 0; i< clients.length; i++){
	if (clients[i] != null)
	clients[i].sock.write("*6|"+bodylist+"*");
	}
}

function BroadcastClientJoin(id){
	for (var i = 0; i< clients.length; i++){
	if (clients[i] != null && i != id)
	clients[i].sock.write("*1,"+id+","+clients[id].pos+","+clients[id].rot+"*");
	}
}

function BroadcastClientLeave(id){
	for (var i = 0; i< clients.length; i++){
	if (clients[i] != null && i != id)
	clients[i].sock.write("*0,"+id+"*");
	}
	clients[id] = null;
}

function UpdateClient(ip,port,pos,rot,moving){
	for (var i = 0; i<clients.length; i++){
		if (clients[i] != null)
		if (clients[i].ipport == ip+port){
			clients[i].pos = pos;
			clients[i].rot = rot;
			clients[i].moving = moving;
			//BroadcastClient(i);
		}
	}
}


function InsertClient(sock,ip,port,name){
var arraynull = -1;
for (var i = 0; i<clients.length; i++){
	if(clients[i] == null){
	arraynull = i;
	break;
	}
}
if (arraynull > -1){
	clients[arraynull] = new Client (sock,ip,port,arraynull,name);
	clients[arraynull].init();
	//BroadcastServerState(clients[arraynull].id);
	//BroadcastClientJoin(arraynull);
}else{
	var l = clients.length;
	clients[l] = new Client (sock,ip,port,l,name);
	clients[l].init();
	//BroadcastServerState(clients[l].id);
	//BroadcastClientJoin(l);
}
console.log("Number of Clients: "+clients.length);
}

function DeleteClient(ip,port){
	for (var i = 0; i<clients.length; i++){
		if (clients[i] != null)
		if (clients[i].ip+clients[i].port == ip+port){
		console.log(clients[i].name+" Disconnected")
		BroadcastClientLeave(i);
		break;
		}
	}
}


function Body (id){
this.type = 0;
this.id = id;
this.parts = new Array();

this.setParts = function (){
	this.type = 2;
this.parts.push (new Joint("FootLeft"));
this.parts.push (new Joint("AnkleLeft"));
this.parts.push (new Joint("KneeLeft"));
this.parts.push (new Joint("HipLeft"));

this.parts.push (new Joint("FootRight"));
this.parts.push (new Joint("AnkleRight"));
this.parts.push (new Joint("KneeRight"));
this.parts.push (new Joint("HipRight"));

this.parts.push (new Joint("HandTipLeft"));
this.parts.push (new Joint("ThumbLeft"));
this.parts.push (new Joint("HandLeft"));
this.parts.push (new Joint("WristLeft"));
this.parts.push (new Joint("ElbowLeft"));
this.parts.push (new Joint("ShoulderLeft"));

this.parts.push (new Joint("HandTipRight"));
this.parts.push (new Joint("ThumbRight"));
this.parts.push (new Joint("HandRight"));
this.parts.push (new Joint("WristRight"));
this.parts.push (new Joint("ElbowRight"));
this.parts.push (new Joint("ShoulderRight"));

this.parts.push (new Joint("SpineBase"));
this.parts.push (new Joint("SpineMid"));
this.parts.push (new Joint("SpineShoulder"));
this.parts.push (new Joint("Neck"));
this.parts.push (new Joint("Head"));

}

this.setPartsV1 = function (){
	this.type = 1;
this.parts.push (new Joint("15"));
this.parts.push (new Joint("14"));
this.parts.push (new Joint("13"));
this.parts.push (new Joint("12"));

this.parts.push (new Joint("19"));
this.parts.push (new Joint("18"));
this.parts.push (new Joint("17"));
this.parts.push (new Joint("16"));

//this.parts.push (new Joint("HandTipLeft"));
//this.parts.push (new Joint("ThumbLeft"));
this.parts.push (new Joint("7"));
this.parts.push (new Joint("6"));
this.parts.push (new Joint("5"));
this.parts.push (new Joint("4"));

//this.parts.push (new Joint("HandTipRight"));
//this.parts.push (new Joint("ThumbRight"));
this.parts.push (new Joint("11"));
this.parts.push (new Joint("10"));
this.parts.push (new Joint("9"));
this.parts.push (new Joint("8"));

this.parts.push (new Joint("0"));
this.parts.push (new Joint("1"));
this.parts.push (new Joint("2"));
this.parts.push (new Joint("3"));
//this.parts.push (new Joint("Head"));

}


}


function Joint (id){
this.id = id;
this.position = new Vector3(3,3,3);
this.rotation = new Quaternion(4,4,4,4);
this.infered = 0;

}

function Vector3 (x,y,z){
this.x = x;
this.y = y;
this.z = z;
}

function Quaternion (x,y,z,w){
this.x = x;
this.y = y;
this.z = z;
this.w = w;
}


function KinectBroadcaster (sock,ip,port,id,name){
this.sock = sock;
this.ip = ip;
this.port = port;
this.ipport = ip+port;
this.id = id;
this.name = name;



}


function Client (sock,ip,port,id,name){
this.sock = sock;
this.ip = ip;
this.port = port;
this.ipport = ip+port;
this.id = id;
this.name = name;
this.moving = "0";
this.pos = "0,0,0";
this.rot = "0,0,0";
this.justjoined = true;
this.init = function(){
BroadcastBodies(sock);
console.log("Login "+name);
}

}

function BroadcastBodies (sock){
 for (var i = 0; i < bodies.length; i++){
	if (bodies[i]){
		sock.write("*1|"+bodies[i].id+"*");
	}
 }
}


function BroadcastServerState(id){
	for (var i = 0; i< clients.length; i++){
	if (i != id && clients[i] != null)
	clients[id].sock.write("*1,"+i+","+clients[i].pos+","+clients[i].rot+"*");
	}
}





// =================================================== END FUNCTIONS

// --------------------------------------------------- STATUS MESSAGES

console.log("Listening for clients at "+HOST + ":" + DEVICEPORT);

// =================================================== STATUS MESSAGES
