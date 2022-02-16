import socket
import sqlite3

host_list = {}
s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)


def start_server():
    global s
    s.bind(('0.0.0.0', 8888))
    print('server started')
    s.listen()
    c, c_address = s.accept()
    while True:
        data = c.recv(1024).decode().split()
        if data[0] == 'LOGIN':
            print(data)
            c.send("Accepted".encode())
        if data[0] == 'HOST':
            print(data)
            try:
                start_host(c_address, data[1], data[2])
                c.send("Accepted".encode())
            except:
                c.send("An Error Occurred".encode())
        if data[0] == 'REFRESH':
            print(data)
            # try:
            c.send("Accepted".encode())
            send_active_servers(c)
            # except:
               # c.send("An Error Occurred".encode())


def start_host(c_address, name, password):
    global host_list
    print('entered def')
    host_list[name] = [name, password, False, c_address]
    print(name + " opened a new room: " + str(host_list[name]))
    for key in host_list.keys():
        print(key + "'s room: " + str(host_list[key]))


def send_active_servers(c):
    global host_list, s
    str_send = ''
    for key in host_list:
        str_send += str(host_list[key]) + '\n'

    c.send(str_send.encode())


def connect_to_host(name, password):
    global host_list, s


start_server()
