require('./utils');

require('dotenv').config();
const express = require('express');
const session = require('express-session');
const MongoStore = require('connect-mongo');
const bcrypt = require('bcrypt');
const saltRounds = 12;



const database = include('databaseConnection');
const db_utils = include('database/db_utils');
const db_users = include('database/users');

const success = db_utils.printMySQLVersion();

const port = process.env.PORT || 3000;

const app = express();

const expireTime = 60 * 60 * 1000; //expires after 1 day  (hours * minutes * seconds * millis)

/* secret information section */
const mongodb_user = process.env.MONGODB_USER;
const mongodb_password = process.env.MONGODB_PASSWORD;;
const mongodb_session_secret = process.env.MONGODB_SESSION_SECRET;;

const node_session_secret = process.env.NODE_SESSION_SECRET;
/* END secret section */

// set view engine to ejs
app.set('view engine', 'ejs');

app.use(express.urlencoded({
    extended: false
}));

var mongoStore = MongoStore.create({
    mongoUrl: `mongodb+srv://${mongodb_user}:${mongodb_password}@cluster0.nbfzg7h.mongodb.net/?retryWrites=true&w=majority`,
    crypto: {
        secret: mongodb_session_secret
    }
})

app.use(session({
    secret: node_session_secret,
    store: mongoStore, //default is memory store 
    saveUninitialized: false,
    resave: true,
    cookie: {
        maxAge: 1000 * 60 * 60 // 1 hour
    }
}));


app.post("/login", async (req, res) => {
    const user = req.body.username;
    const password = req.body.password;
    const results = await db_users.getUser({ user: user });
  
    if (results) {
      if (results.length === 1) {
        // Ensure there is exactly one matching user
        const storedHashedPassword = results[0].password;
  
        // Compare the user-entered password with the stored hashed password
        if (bcrypt.compareSync(password, storedHashedPassword)) {
          req.session.authenticated = true;
          req.session.user = results[0].username;
          req.session.user_id = results[0].user_id;
          req.session.cookie.maxAge = expireTime;
          res.redirect("/home?image=true");
          return;
          // Handle the login success case here
        } else {
          console.log("Invalid password");
          // Handle the invalid password case here
        }
      } else {
        console.log(
          "Invalid number of users matched: " + results.length + " (expected 1)."
        );
        // Handle the case where multiple users match the query
      }
    }
    res.redirect("/login?invalid=true");
  });