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
const db_users = include('database/users');

const success = db_utils.printMySQLVersion();

const port = process.env.PORT || 3000;

const app = express();

const expireTime = 60 * 60 * 1000; //expires after 1 day  (hours * minutes * seconds * millis)
app.use(express.json()); // Add this line to parse JSON requests

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
  console.log("inside log in")
  console.log(req.body.email);
  const user = req.body.email;
  const password = req.body.password;
  const results = await db_users.getUser({
    user: user
  });

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

app.listen(PORT, () => {
  console.log(`Server listening on ${PORT}`);
});


app.post("/signup", async (req, res) => {
  console.log(req.body);

  const {
    email,
    password
  } = req.body;

  console.log("Email: " + email);
  console.log("Password: " + password);

  // Now you can use 'email' and 'password' as needed in your server logic
});



// app.post("/signup", async (req, res) => {
//   console.log("Body items " + req.body.email);
//   const {
//     email,
//     password
//   } = req.body;
//   console.log("Email" + email)
//   console.log('Password ' + password)

// const checkPassword = () => {
//   if (password.length < 10) {
//     passwordValidClass = "is-invalid";
//     passwordInvalidMessage = "Password must be at least 10 characters";
//     return true;
//   }
//   if (!password.match(/[a-z]/)) {
//     passwordValidClass = "is-invalid";
//     passwordInvalidMessage = "Password must contain a lowercase letter";
//     return true;
//   }
//   if (!password.match(/[A-Z]/)) {
//     passwordValidClass = "is-invalid";
//     passwordInvalidMessage = "Password must contain an uppercase letter";
//     return true;
//   }
//   if (!password.match(/[|\\/~^:,;?!&%$@*+]/)) {
//     passwordValidClass = "is-invalid";
//     passwordInvalidMessage = "Password must contain a special character";
//     return true;
//   }
//   return false;
// };

// const checkEmail = () => {
//   if (!email) {
//     userValidClass = "is-invalid";
//     userInvalidMessage = "Please enter your username";
//     return true;
//   }
//   const userExists = db_users.getUser({
//     user: username
//   });
//   if (userExists.length <= 0) {
//     userValidClass = "is-invalid";
//     userInvalidMessage = "Username already exists";
//     return true;
//   }
//   return false;
// };

// const isPasswordInvalid = checkPassword();
// const isUserInvalid = checkEmail();

// if (isPasswordInvalid || isUserInvalid) {
//   return;
// }
// try {
//   const hashedPassword = await bcrypt.hash(password, saltRounds);
//   const success = await db_users.createUser({
//     username: username,
//     password: hashedPassword,
//   });
//   if (success) {
//     console.log("User created successfully");
//     res.redirect("/login");
//   } else {
//     console.error("YIkes Failed to create user");
//     res.redirect("/signUp");
//   }
// } catch (error) {
//   console.error("Error while creating user:", error);
//   res.redirect("/signUp");
// }
// });