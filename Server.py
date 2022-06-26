import socket
import threading
import sqlite3
import smtplib
import random
from email.message import EmailMessage

def client_connection(c, c_address):
    print("connected: " + str(c_address))
    global s
    a = ""
    pid = 0
    tries = 3
    logged_in = False
    while True:
        data = c.recv(1024).decode()
        if "|" in data:
            print(data)
            data = data.split('|')
            if data[0] == "VERIFY":
                try:
                    if tries > 0 and not logged_in:
                        print("so far so good")
                        logged_in = verify(data[1], c, a, tries)
                        c.send("accepted".encode())
                except Exception as exc:
                    c.send(("An Error Occurred " + str(type(exc)) + str(exc)).encode())
            elif data[0] == 'REGISTER' and not logged_in:
                data = data[1].split('#')
                try:
                    a = register(data[0], data[1], data[2])
                    c.send("accepted".encode())
                except Exception as exc:
                    c.send(("An Error Occurred "+str(type(exc)) + str(exc)).encode())
            elif data[0] == 'LOGIN' and not logged_in:
                data = data[1].split('#')
                try:
                    tpl = login(data[0], data[1], c)
                    print(tpl)
                    if tpl != None:
                        a = tpl[0]
                        pid = tpl[1]
                    c.send("Accepted".encode())
                    print("The code is " + str(a))
                except Exception as exc:
                    print(exc)
                    c.send(("An Error Occurred "+str(type(exc)) + str(exc)).encode())
            elif data[0] == 'START':
                c.send("Accepted".encode())
            elif data[0] == 'HOST' and logged_in:
                data = data[1].split('#')
                try:
                    host(c_address, data[0], data[1], data[2])
                    c.send("Accepted".encode())
                except Exception as exc:
                    c.send(("An Error Occurred "+str(type(exc)) + str(exc)).encode())
            elif data[0] == 'REFRESH' and logged_in:
                try:
                    c.send("Accepted".encode())
                    refresh(c)
                except Exception as exc:
                    c.send(("An Error Occurred "+str(type(exc)) + str(exc)).encode())
            elif data[0] == 'JOIN' and logged_in:
                data = data[1].split('#')
                try:
                    join(data[0], data[1], c, c_address)
                    c.send("Accepted".encode())
                except Exception as exc:
                    print("An Error Occurred "+str(type(exc)) + str(exc))
                    c.send(("An Error Occurred "+str(type(exc)) + str(exc)).encode())
            elif data[0] == "WIN" and logged_in:
                win(data[1])
                c.send("Accepted".encode())

                # print("An Error Occurred " + str(type(exc)) + str(exc))
                # c.send(("An Error Occurred " + str(type(exc)) + str(exc)).encode())
            elif data[0] == 'CLOSE':
                print("ima")
                close(c, pid)
                break


def login(name, password, c):
    print(players_connected)
    global s
    crsr.execute("SELECT Password, ID FROM USERS WHERE Name = ?", (name,))
    data = crsr.fetchall()
    print(data[0])
    if data == []:
        c.send("0".encode())
    elif data[0][0] == password and not data[0][1] in players_connected:
        crsr.execute("SELECT Email FROM USERS WHERE Name = ?", (name,))
        cfa = crsr.fetchall()[0]
        email = cfa[0]

        c.send(str(data[0][1]).encode())
        players_connected.append(data[0][1])
        a = random.randint(1000, 10000)
        msg = EmailMessage()
        message = "Thank you for choosing to play Murder Of The Universe!\nThe security code is " + str(a)
        msg.set_content(message)

        msg['Subject'] = "Your Security Code For Logging Into Murder Of The Universe"
        msg['From'] = "MurderOfTheUniverse@outlook.com"
        msg['To'] = email
        emailServer.send_message(msg)
        print(players_connected)
        return tuple((a, data[0][1]))
    else:
        c.send("0".encode())


def register(name, password, email):
    global s
    crsr.execute("INSERT INTO Users(Name, Password, Email, Wins) values(?, ?, ?,0)", (name, password, email))
    conn.commit()

    crsr.execute("SELECT Email FROM USERS WHERE Name = ?", (name,))
    email = crsr.fetchall()[0][0]

    a = random.randint(1000, 10000)
    msg = EmailMessage()
    message = "Thank you for choosing to play Murder Of The Universe!\nThe security code is " + str(a)
    msg.set_content(message)

    msg['Subject'] = "Your Security Code For Logging Into Murder Of The Universe"
    msg['From'] = "MurderOfTheUniverse@outlook.com"
    msg['To'] = email
    emailServer.send_message(msg)
    return a


def verify(code, c, a, tries):
    print("a = " + str(a))
    if int(code) == a:
        print(a)
        c.send("1".encode())
        return True
    else:
        c.send(str(1-tries).encode())
        tries -= 1
        return False


def host(c_address, name, password, username):
    global host_list
    host_list[name] = [password, username, False, c_address]
    print(username + " opened a new room: " + name + "- " + str(host_list[name]))


def refresh(c):
    global host_list, s
    str_send = ''
    for key in host_list.keys():
        if not host_list[key][2] and key != "":
            str_send += key + '#'
    str_send += "#"
    c.send(str_send.encode())


def join(name, password, c, c_address):
    global host_list, s
    if host_list[name][0] == password:
        print(host_list[name][3])
        host_list[name][2] = True
        c.send((str(host_list[name][3]) + "#" + str(c_address)).encode())
    else:
        c.send('0'.encode())


def win(ids):
    id_list = ids[0:-1:].split("#")
    print(id_list)
    for i in id_list:
        crsr.execute("SELECT Wins FROM USERS WHERE ID = ?", (i,))
        wins = crsr.fetchall()[0][0]

        crsr.execute("UPDATE Users SET Wins=? WHERE ID = ?", (wins + 1, i))


def close(c, pid):
    c.close()
    if pid in players_connected:
        print("players connected: " + str(players_connected))
        players_connected.remove(pid)



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


host_list = {}
players_connected = []
s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
conn = sqlite3.connect("Murder Of The Universe.db", check_same_thread=False)
crsr = conn.cursor()
emailServer = smtplib.SMTP(host="smtp.office365.com", port=587)
emailServer.starttls()
emailServer.login("MurderOfTheUniverse@outlook.com", "Murd3r0fTh3Un1v3rs3")
main = threading.Thread(target=start_server)
main.start()

