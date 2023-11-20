const database = include("mySQLDatabaseConnection");

async function createUser(postData) {
    let createUserSQL = `
		INSERT INTO user
		(username, password)
		VALUES
		(:username,  :passwordHash);
	`;

    let params = {
        username: postData.username,
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
		FROM user
		WHERE username = :username;
	`;
    let params = {
        username: postData.user,
    };
    try {
        const results = await database.query(getUsersSQL, params);
        if (results) {
            return results[0];
        }
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