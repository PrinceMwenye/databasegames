const database = include("mySQLDatabaseConnection");

async function createUser(postData) {
    let createUserSQL = `
		INSERT INTO SnakeUser
		(username, password)
		VALUES
		(:useremail,  :passwordHash);
	`;

    let params = {
        useremail: postData.email,
        passwordHash: postData.password,
    };

    try {
        await database.query(createUserSQL, params);
        return true;
    } catch (err) {
        console.log("Error inserting user");
        console.log(err);
        return false;
    }
}

async function getUser(postData) {
    console.log("Checking users in database " + postData);
    console.log(postData);
    let getUsersSQL = `
		SELECT *
		FROM SnakeUser
		WHERE username = :username;
	`;
    let params = {
        username: postData,
    };
    try {
        const results = await database.query(getUsersSQL, params);
        if (results[0].length > 0) {
            return true;
        } else {
            return false;
        };

    } catch (err) {
        console.log("Error getting users");
        console.log(err);
        return false;
    }
}

module.exports = {
    createUser,
    getUser
};