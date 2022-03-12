import socket
import threading
import sqlite3

host_list = {}
s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)


def client_connection(c, c_address):
    global s
    while True:
        data = c.recv(1024).decode()
        if data != "":
            data = data.split('|')
            if data[0] == 'LOGIN':
                print(data)
                c.send("Accepted".encode())
            if data[0] == 'HOST':
                print(data)
                data = data[1].split('#')
                try:
                    start_host(c_address, data[0], data[1])
                    c.send("Accepted".encode())
                except:
                    c.send("An Error Occurred".encode())
            if data[0] == 'REFRESH':
                print(data)
                try:
                    c.send("Accepted".encode())
                    send_active_servers(c)
                except:
                    c.send("An Error Occurred".encode())


def start_server():
    global s
    s.bind(('0.0.0.0', 8888))
    print('server started')
    s.listen()
    t = []
    while True:
        c, c_address = s.accept()
        t.append(threading.Thread(target=client_connection(c, c_address)).start())


def start_host(c_address, name, password):
    global host_list
    print('entered def')
    host_list[name] = [password, False, c_address]
    print("a player opened a new room: " + str(host_list[name]))
    for key in host_list.keys():
        print(key + "'s room: " + str(host_list[key]))


def send_active_servers(c):
    global host_list, s
    str_send = ''
    for key in host_list:
        if not host_list[key][1] and key != "":
            str_send += key + '#'
    str_send += "end"
    c.send(str_send.encode())


def connect_to_host(name, password):
    global host_list, s


start_server()
