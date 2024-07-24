import { useState } from "react";
import { useEffect } from "react";
import config from "../config.json";

interface NavQuestionsProps {
    group: string;
    name: string;
    setActiveQuestion: (question: number) => void;
}

export const NavQuestions: React.FC<NavQuestionsProps> = ({ group, name, setActiveQuestion }) => {
    const[submissionQuestions, setSubmissionQuestions] = useState([]);
    const questionsRoute = 
        `http://${config.server_host}:${config.server_port}/Submission/GetFilledQuestions?group=${group}&name=${name}`;

    useEffect(() =>  {
    const fetchQuestions = async () => {
        await fetch(questionsRoute)
        .then(response => response.json())
        .then(data => {
            setSubmissionQuestions(data);
        })
        .catch(error => {
            console.error('Error fetching questions:', error);
        });
    }
        fetchQuestions();
    }, [questionsRoute]);

    return (
        <ul>
            {submissionQuestions.map((name, index) => 
            <li key={index}> 
                <button className= "pl-16 text-white text-sans text-lg w-full text-left hover:bg-lighterTeal" onClick = {() => setActiveQuestion(name)}>
                    <span className="pl-8">{"Q" + name}</span>
                </button>
            </li>)}
        </ul>
    )
}