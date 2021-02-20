#!/bin/bash
echo "Building FNA3D for Linux or macOS, please be patient. :)"
(cd Nez/Modules/FNA3D && cmake . ; make -j$(nproc))
echo "Done!"