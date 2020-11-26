#include <iostream>
#include <WS2tcpip.h>
#include <string>
#include <fstream>
#include <streambuf>
#include <iomanip>
#include "Controller.h"
#include <vector>
#include <chrono>
#include <thread>
#include <cstdio>
#include <regex>
#pragma comment(lib, "ws2_32.lib")

#define PORT			54000
#define IPADDRESS		"127.0.0.1"
#define PHASE_ONE		"phase_one.json"
#define PHASE_TWO		"phase_two.json"
#define PHASE_THREE		"phase_three.json"
#define PHASE_FOUR		"phase_four.json"
#define PHASE_RED		"phase_red.json"

const std::vector<std::string> cycle{ PHASE_ONE, PHASE_RED, PHASE_TWO, PHASE_RED, PHASE_THREE, PHASE_RED, PHASE_FOUR, PHASE_RED, };
bool connected = false;
char hdr = 'y';
void sendJson(const SOCKET& socket, const char file[], const char hdr);

/// <summary>
/// Initializes listening socket, accepts client, inits socket, sends json data
/// </summary>
int main()
{
	std::cout << "Enter server delay in milliseconds: (1000 milliseconds = 1 second)" << std::endl;

	int time;
	std::cin >> time;

	// Initialize winsock
	WSADATA wsData;
	WORD ver = MAKEWORD(2, 2);

	int wsOk = WSAStartup(ver, &wsData);
	if (wsOk != 0) // Handle error
	{
		std::cerr << "Can't Initialize winsock! Quitting" << std::endl;
		return -1;
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

	SOCKET socket = accept(listening, (sockaddr*)&client, &clientSize);
	if (socket == INVALID_SOCKET) // Handle error
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

	connected = true;

	// Close listening socket
	closesocket(listening);

	// While loop: send json once in a while
	
	while (connected)
	{
			for (auto phase : cycle)
			{
				std::this_thread::sleep_for(std::chrono::milliseconds(time));
				sendJson(socket, phase.c_str());
			}		
	}

	// Close the socket
	closesocket(socket);

	// Cleanup winsock
	WSACleanup();

	std::getchar();
	std::getchar();

	return 0;
}


int counter = 0;

/// <summary>
/// Sends json data over socket
/// with header or wihtout header
/// </summary>
/// <param name="socket">SOCKET</param>
/// <param name="file">Filename</param>
/// <param name="hdr">with header: 'y' without header: 'no'</param>
void sendJson(const SOCKET& socket, const char file[])
{
	std::ifstream f;
	f.open(file);
	
	if (!f)
	{
		std::cerr << "Can't open file" << std::endl;
	}
	
	std::string content((std::istreambuf_iterator<char>(f)),
		(std::istreambuf_iterator<char>()));

	content.erase(remove(content.begin(), content.end(), ' '), content.end());
	content.erase(remove(content.begin(), content.end(), '\n'), content.end());
	content.erase(remove(content.begin(), content.end(), '\t'), content.end());

	std::string package = "";

	std::string header = std::to_string(content.length()) + ":";

	package = header + content;

	if (connected)
	{
		//send data over socket
		int bytesSend = send(socket, package.c_str(), package.size(), 0);
		if (bytesSend == SOCKET_ERROR)
		{
			std::cerr << "Client disconnected!" << std::endl;
			counter = 0;
			connected = false;
			main();
		}
		else
		{
			counter++;

			std::cout << "SENT: \n" << package << std::endl;
			std::cout << "SENT_AMOUNT: " << counter << std::endl;
		}

	}
	
}

/// <summary>
/// Receives data over socket and prints to Output
/// </summary>
/// <param name="buf">buffer to store data</param>
/// <param name="socket">SOCKET</param>
//void receiveJson(char buf[4096], const SOCKET& socket)
//{
//	ZeroMemory(buf, 4096);
//
//	// Wait for client to send data
//	int bytesReceived = recv(socket, buf, 4096, 0);
//	if (bytesReceived == SOCKET_ERROR) // Handle error
//	{
//		std::cerr << "Error in recv(). Quitting!" << std::endl;
//	}
//
//	if (bytesReceived == 0) // Handle error
//	{
//		std::cout << "Client disconnected " << std::endl;
//	}
//
//	std::cout << "CLIENT> " << std::string(buf, 0, bytesReceived) << std::endl;
//}

