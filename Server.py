import socket
import threading
import sqlite3
import smtplib
import random


def client_connection(c, c_address):
    global s
    a = ""
    tries = 3
    while True:
        data = c.recv(1024).decode()
        if data != "":
            data = data.split('|')
            if data[0] == "VERIFY":
                print(data)
                try:
                    if tries > 0:
                        tries += verify(data[1], c, a, tries)
                        c.send("accepted".encode())
                except Exception as exc:
                    c.send(("An Error Occurred " + str(type(exc)) + str(exc)).encode())
            if data[0] == 'REGISTER':
                print(data)
                data = data[1].split('#')
                try:
                    register(data[0], data[1], data[2])
                    c.send("accepted".encode())
                except Exception as exc:
                    c.send(("An Error Occurred "+str(type(exc)) + str(exc)).encode())
            if data[0] == 'LOGIN':
                print(data)
                data = data[1].split('#')
                try:
                    a = login(data[0], data[1], c)
                    c.send("Accepted".encode())
                    print(a)
                except Exception as exc:
                    c.send(("An Error Occurred "+str(type(exc)) + str(exc)).encode())
            if data[0] == 'START':
                print(data)
                c.send("Accepted".encode())
            if data[0] == 'HOST':
                print(data)
                data = data[1].split('#')
                try:
                    host(c_address, data[0], data[1])
                    c.send("Accepted".encode())
                except:
                    c.send("An Error Occurred".encode())
            if data[0] == 'REFRESH':
                print(data)
                try:
                    c.send("Accepted".encode())
                    refresh(c)
                except:
                    c.send("An Error Occurred".encode())


def login(name, password, c):
    global s
    crsr.execute("SELECT Password FROM USERS WHERE Name = ?", (name,))
    data = crsr.fetchall()
    print(data)
    if data == []:
        print("wrong user")
        c.send("0".encode())
    elif data[0][0] == password:
        print("correct")
        c.send("1".encode())
        with smtplib.SMTP('smtp.gmail.com', 587) as smtp:
            smtp.ehlo()
            smtp.starttls()
            smtp.ehlo()
            crsr.execute("SELECT Email FROM USERS WHERE Name = ?", (name,))
            email = crsr.fetchall()[0][0]
            smtp.login("revengeofthedreamers3@gmail.com", "R3V3NG30fTh3Dr3m3rs")

            a = random.randint(1000, 10000)

            subject = 'Confirmation Code'
            body = "Hello,\nThe verification code is: " + str(a) + "\nHave Fun!"

            msg = f'Subject: {subject}\n\n{body}'

            smtp.sendmail("revengeofthedreamers3@gmail.com", email, msg)
            return a
    else:
        print("wrong pass")
        c.send("0".encode())


def register(name, password, email):
    global s
    crsr.execute("INSERT INTO Users(Name, Password, Email, Wins) values(?, ?, ?,0)", (name, password, email))


def verify(code, c, a, tries):

    if int(code) == a:
        print("success")
        c.send("1".encode())
        return 0
    else:
        print("failed")
        c.send(str(1-tries).encode())
        return -1


def start_server():
    global s
    s.bind(('0.0.0.0', 8888))
    print('server started')
    s.listen(999)
    t = []

    while True:
        try:
            print("listening")
            c, c_address = s.accept()
            temp = threading.Thread(target=client_connection, args=(c, c_address))
            temp.start()
            t.append(temp)
        except Exception as exc:
            print("Server Full... try again " + str(exc))


def host(c_address, name, password):
    global host_list
    print('entered def')
    host_list[name] = [password, False, c_address]
    print("a player opened a new room: " + str(host_list[name]))
    for key in host_list.keys():
        print(key + "'s room: " + str(host_list[key]))


def refresh(c):
    global host_list, s
    str_send = ''
    for key in host_list:
        if not host_list[key][1] and key != "":
            str_send += key + '#'
    str_send += "end"
    c.send(str_send.encode())


def connect(name, password, c):
    global host_list, s


host_list = {}
s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
conn = sqlite3.connect("Revenge Of The Dreamers III.db", check_same_thread=False)
crsr = conn.cursor()
main = threading.Thread(target=start_server)
main.start()

