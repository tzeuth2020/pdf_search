"use client";

import Image from "next/image";
import { useState } from "react";
import { useEffect } from "react";
import { NavNames } from "../components/NavNames";
import { DocText } from "@/components/DocText";
import { SearchBar } from "@/components/SearchBar";
import  Modal from 'react-modal';
import config from "../config.json";
import { UploadBox } from "@/components/FileUpload";

Modal.setAppElement('#root');


export default function Home() {
  const [groupNames, setGroupNames] = useState<string[]>([]);
  const [activeGroup, setActiveGroup] = useState(-1);
  const [activeName, setActiveName] = useState<string | undefined>(undefined);
  const [activeQuestion, setActiveQuestion] = useState(0);
  const [activePattern, setActivePattern] = useState("");
  const [modalIsOpen, setModalIsOpen] = useState(false);


  const groupsRoute = `http://${config.server_host}:${config.server_port}/Submission/GroupNames`;

  const fetchNames = async () => {
    await fetch(groupsRoute)
      .then(response => response.json())
      .then(data => {
          setGroupNames(data);
      })
      .catch(error => {
          error.name !== 'AbortError' && console.error('Error fetching names:', error);
      });
  }  

  useEffect(() =>  {
      fetchNames();
    }, [groupsRoute]);

  useEffect(() => {
      setActivePattern("");
    }, [activeGroup, activeName, activeQuestion]);
 
  const customStyles = {
    content: {
      top: '50%',
      left: '50%',
      right: 'auto',
      bottom: 'auto',
      boxShadow: '0 50px 85px -9px rgb(0 0 0 / 0.35), 10px 25px 30px -6px rgb(0 0 0 / 0.15)',
      borderRadius: '1.0rem',
      marginRight: '-50%',
      transform: 'translate(-50%, -50%)',
      width: 'min(380px, 100%)', 
      height: '40%', 
    },
  };
  
  return (
    <main className="flex min-h-screen flex-col items-center" >
      <div className="bg-teal h-16 w-full flex items-center justify-start">
          <button className="bg-teal ml-24 my-1 text-white text-xl font-bold font-sans px-4 py-2 justify-center border-darkTeal border-2 rounded-full hover:bg-lighterTeal" onClick={() =>setModalIsOpen(true)}>Upload</button>
          <Modal 
            isOpen={modalIsOpen}
            onRequestClose={() => setModalIsOpen(false)}
            style={customStyles}
          >
            <div className = "items-center, justify-center h-full">
              <UploadBox groupNames={groupNames} fetchGroupNames={() => fetchNames()} setModalIsOpen={setModalIsOpen}/>
            </div>
          </Modal>
      </div>

      <div className="flex w-full flex-1  h-full" id="root">
        <div className="w-1/4 bg-lightTeal flex flex-col">
          <ul className="flex-1">
            {groupNames.map((name, index) => 
              <li className=" w-full justify-items-start " key={index}>
                <button className="pl-12 text-white text-sans text-lg w-full text-left hover:bg-lighterTeal" onClick={() => {
                  setActiveQuestion(0); setActiveName(undefined); setActiveGroup(index)}}>{name}</button>
                {activeGroup === index && 
                <div>
                  <NavNames group={name} activeName={activeName} setActiveName={setActiveName} setActiveQuestion={setActiveQuestion}/>
                </div>
                }
              </li> )}
          </ul>
        </div>

        <div className="w-3/4 p-6 bg-slate-200">
        {activeGroup !== -1 && activeName && (
          <div>
            <div className="w-full">
              <SearchBar pattern= {activePattern} setPattern={setActivePattern} />
            </div>

            <div className="w-full">
              <DocText group = {groupNames[activeGroup]} name = {activeName} question = {activeQuestion} pattern = {activePattern} /> 
            </div>
          </div>
        )}
        </div> 
     </div>


  
    </main>
  );
}
