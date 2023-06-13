import socket

from lib.client import Client

HOST = ''
PORT = 12345


def start_server():
    server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    server_socket.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
    try:
        # Bind the socket to the specified host and port
        server_socket.bind((HOST, PORT))
        print("Server started on {}:{}".format(HOST, PORT))

        while True:
            server_socket.listen()
            client_socket, client_address = server_socket.accept()
            print("Client connected:", client_address)
            client = Client(client_socket)
            client.start()

    except KeyboardInterrupt:
        print("Server stopped by the user")
    finally:
        server_socket.close()


if __name__ == '__main__':
    start_server()
