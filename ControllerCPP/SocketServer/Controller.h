#pragma once

void sendJson(const SOCKET& socket, const char file[]);

void receiveJson(char  buf[4096], const SOCKET& socket);
