import React from 'react';
import { Link } from 'react-router-dom';

function Home() {
    return (
        <div>
            <h1>CRUD Application</h1>
            <p>��ʯ�ƹ���ꟳ���� An example app showing how to list, add, edit and delete user records with React and the React Hook Form library.</p>
            <p><Link to="users">&gt;&gt; Manage Users</Link></p>
        </div>
    );
}

export default Home;