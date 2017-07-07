#pragma once

#include <cstdlib>
#include <bond/core/exception.h>

namespace detail
{

#define MAX_ALLOC_SIZE 0x00200000

    template <typename T = char>
    struct Mallocator {
        typedef T value_type;
        Mallocator() = default;
        template <class U> Mallocator(const Mallocator<U>&) {}
       
        T* allocate(std::size_t n)
        {
            if (n > MAX_ALLOC_SIZE)
            {
                bond::ExcessiveAllocationException();
            }
            return static_cast<T*>(std::malloc(n * sizeof(T)));
        }

        void deallocate(T* p, std::size_t) { std::free(p); }

        template<class Other>
        struct rebind
        {
            // convert this type to Mallocator<Other>
            typedef Mallocator<Other> other;
        };
    };
    template <class T, class U>
    bool operator==(const Mallocator<T>&, const Mallocator<U>&) { return true; }
    template <class T, class U>
    bool operator!=(const Mallocator<T>&, const Mallocator<U>&) { return false; }

} // namespace detail


typedef detail::Mallocator<> Mallocator;

