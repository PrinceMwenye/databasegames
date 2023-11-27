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
    console.log("Checking users in database" + postData);
    console.log(postData);
    let getUsersSQL = `
		SELECT *
		FROM SnakeUser
		WHERE username = :useremail;
	`;
    let params = {
        useremail: postData.email,
    };
    try {
        const results = await database.query(getUsersSQL, params);
        return results.length > 0;

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