require('./utils');

require('dotenv').config();
const express = require('express');
const session = require('express-session');
const MongoStore = require('connect-mongo');
const bcrypt = require('bcrypt');
const saltRounds = 12;
const PORT = process.env.PORT || 3000;



const database = include('mySQLDatabaseConnection');
const db_utils = include('./database/db_utils.js');
const db_users = include('database/db_users');

const success = db_utils.printMySQLVersion();

const port = process.env.PORT || 3000;

const app = express();

const expireTime = 60 * 60 * 1000; //expires after 1 day  (hours * minutes * seconds * millis)
app.use(express.json());

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
  console.log("inside log in");
  console.log(req.body.email);

  const email = req.body.email;
  const password = req.body.password;

  const results = await db_users.getUserLogIn({
    email,
  });

  if (results) {
    if (results.length === 1) {
      const storedHashedPassword = results[0].password;

      if (bcrypt.compareSync(password, storedHashedPassword)) {
        // Login successful
        req.session.authenticated = true;
        req.session.user = results[0].username;
        req.session.user_id = results[0].user_id;

        // Send a success response to the client
        res.status(200).json({
          message: "Login successful"
        });
        return;
      } else {
        // Invalid password
        console.log("Invalid password");
        res.status(401).json({
          message: "Invalid password"
        });
      }
    } else {
      // Multiple users matched the query
      console.log("Invalid number of users matched: " + results.length + " (expected 1).");
      res.status(500).json({
        message: "Internal server error"
      });
    }
  } else {
    // No user found
    console.log("User not found");
    res.status(404).json({
      message: "User not found"
    });
  }
});



app.post("/signup", async (req, res) => {
  console.log("Body items " + req.body.email);
  const {
    email,
    password
  } = req.body;
  console.log("Email " + email);
  console.log('Password ' + password);

  const userExists = await db_users.getUser(email);
  console.log(userExists);

  if (userExists) {
    console.log("User already exists");
    res.status(409).json({
      message: "User already exists"
    });
  } else {
    try {
      const hashedPassword = await bcrypt.hash(password, saltRounds);
      const success = await db_users.createUser({
        email: email,
        password: hashedPassword,
      });
      if (success) {
        console.log("User created successfully");
        res.status(200).json({
          message: "Signup successful"
        });
      } else {
        console.error("Yikes! Failed to create user");
        res.status(500).json({
          message: "Internal server error"
        });
      }
    } catch (error) {
      console.error("Error while creating user:", error);
      res.status(500).json({
        message: "Internal server error"
      });
    }
  }
});



app.listen(PORT, () => {
  console.log(`Server listening on ${PORT}`);
});