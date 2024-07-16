"use client";

import Image from "next/image";
import { useState } from "react";
import { useEffect } from "react";
import { NavNames } from "../components/NavNames";
import { DocText } from "@/components/DocText";
import { SearchBar } from "@/components/SearchBar";
import config from "../config.json";


export default function Home() {
  const [groupNames, setGroupNames] = useState([]);
  const [activeGroup, setActiveGroup] = useState(-1);
  const [activeName, setActiveName] = useState("undefined");
  const [activeQuestion, setActiveQuestion] = useState(0);
  const [activePattern, setActivePattern] = useState("");

  const groupsRoute = `http://${config.server_host}:${config.server_port}/Submission/GroupNames`;

  useEffect(() =>  {
    const fetchNames = async () => {
      await fetch(groupsRoute)
        .then(response => response.json())
        .then(data => {
            setGroupNames(data);
        })
        .catch(error => {
            console.error('Error fetching names:', error);
        });
    }
      fetchNames();
    }, [groupsRoute]);

  useEffect(() => {
      setActivePattern("");
    }, [activeGroup, activeName, activeQuestion]);
  
  return (
    <main className="flex min-h-screen flex-col items-center justify-between p-24">

      <div className="w-full">
        <ul>
          {groupNames.map((name, index) => 
            <li key={index}>
              <button onClick={() => {
                setActiveQuestion(0); setActiveName("undefined"); setActiveGroup(index)}}>{name}</button>
              {activeGroup === index && 
              <div>
                <NavNames group={name} activeName={activeName} setActiveName={setActiveName} setActiveQuestion={setActiveQuestion}/>
                <p>{activeGroup}  {activeName}  {activeQuestion} {activePattern}</p>
              </div>
              }
            </li> )}
        </ul>
      </div>

      <div className="w-full">

        <SearchBar pattern= {activePattern} setPattern={setActivePattern} />
      </div>

      <div className="w-full">
        <DocText group = {groupNames[activeGroup]} name = {activeName} question = {activeQuestion} pattern = {activePattern} />
      </div>

  
    </main>
  );
}
