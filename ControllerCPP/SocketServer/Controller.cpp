#include <iostream>
#include <WS2tcpip.h>
#include <string>
#include <fstream>
#include <streambuf>
#include <iomanip>
#pragma comment(lib, "ws2_32.lib")

#define PORT				54000
#define IPADDRESS			"127.0.0.1"
#define JSON_CONTROLLER		"jason_controller.json"

/// <summary>
/// Start deze eerst!
/// 
/// SocketServer/controller kan nu (nog) alleen
/// een keer een socket verbinding aan gaan
/// een keer receiven en daarna een keer een geldige json terugsturen (zie source files in solution explorer)
/// </summary>
void main()
{
	// Initialize winsock
	WSADATA wsData;
	WORD ver = MAKEWORD(2, 2);

	int wsOk = WSAStartup(ver, &wsData);
	if (wsOk != 0) // Handle error
	{
		std::cerr << "Can't Initialize winsock! Quitting" << std::endl;
		return;
	}


	// Create a listening socket
	SOCKET listening = socket(AF_INET, SOCK_STREAM, 0);
	if (listening == INVALID_SOCKET) // Handle error
	{
		std::cerr << "Can't create socket! Quitting!" << std::endl;
	}

	// Bind the ip address and port to a socket
	sockaddr_in hint;
	hint.sin_family = AF_INET;
	hint.sin_port = htons(PORT);
	//hint.sin_addr.S_un.S_addr = htonl(INADDR_ANY); // Could also use inet_pton ....
	std::string ipAddress = IPADDRESS;
	inet_pton(AF_INET, ipAddress.c_str(), &hint.sin_addr);

	bind(listening, (sockaddr*)&hint, sizeof(hint));

	// Tell winsock the socket is for listening
	listen(listening, SOMAXCONN);

	std::cout << "Listening..." << std::endl;

	// Wait for a connection
	sockaddr_in client;
	int clientSize = sizeof(client);

	SOCKET clientSocket = accept(listening, (sockaddr*)&client, &clientSize);
	if (clientSocket == INVALID_SOCKET) // Handle error
	{
		std::cerr << "Can't create socket! Quitting!" << std::endl;
	}

	char host[NI_MAXHOST];		// Client's remote name
	char service[NI_MAXSERV];	// Service (i.e port) the client is connected to

	ZeroMemory(host, NI_MAXHOST); // same as memset (host, 0, NI_MAXHOST)
	ZeroMemory(service, NI_MAXSERV);

	if (getnameinfo((sockaddr*)&client, sizeof(client), host, NI_MAXHOST, service, NI_MAXSERV, 0) == 0)
	{
		std::cout << host << " connected on port " << service << std::endl;
	}
	else
	{
		inet_ntop(AF_INET, &client.sin_addr, host, NI_MAXHOST);
		std::cout << host << " connected on port " <<
			ntohs(client.sin_port) << std::endl;
	}

	// Close listening socket
	closesocket(listening);

	// While loop: accept and echo message back to client
	char buf[4096];
	std::string userInput;
	while (true)
	{
		ZeroMemory(buf, 4096);

		//// Wait for client to send data
		int bytesReceived = recv(clientSocket, buf, 4096, 0);
		if (bytesReceived == SOCKET_ERROR) // Handle error
		{
			std::cerr << "Error in recv(). Quitting!" << std::endl;
			break;
		}

		if (bytesReceived == 0) // Handle error
		{
			std::cout << "Client disconnected " << std::endl;
			break;
		}

		std::cout << "CLIENT> " << std::string(buf, 0, bytesReceived) << std::endl;


		std::ifstream f;
		f.open(JSON_CONTROLLER);

		if (f) {
			std::cout << "file exists";
		}
		else {
			std::cout << "file doesn't exist";
		}

		std::string content((std::istreambuf_iterator<char>(f)),
			(std::istreambuf_iterator<char>()));

		send(clientSocket, content.c_str(), content.size(), 0);
		//std::cout << "test" << str.c_str() << "test"<< std::endl;

	}

	// Close the socket
	closesocket(clientSocket);

	// Cleanup winsock
	WSACleanup();
}